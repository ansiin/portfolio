using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Appointments;

public sealed class AppointmentService : IAppointmentService
{
    private readonly ICompanyCrudRepository<Appointment> _appointments;
    private readonly ICompanyCrudRepository<PlanItem> _planItems;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;
    private readonly IAuditLogRepository _audit;

    public AppointmentService(
        ICompanyCrudRepository<Appointment> appointments,
        ICompanyCrudRepository<PlanItem> planItems,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization,
        IAuditLogRepository audit)
    {
        _appointments = appointments;
        _planItems = planItems;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
        _audit = audit;
    }

    public async Task<IReadOnlyCollection<AppointmentItemDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var items = await _appointments.ListAsync(_tenant.Current.CompanyId, ct);
        var planItems = await _planItems.ListAsync(_tenant.Current.CompanyId, ct);
        var planItemLookup = planItems
            .Where(p => !p.IsDeleted)
            .ToDictionary(p => p.Id, p => p.Description);

        return items
            .OrderBy(a => a.StartAt)
            .Select(a => new AppointmentItemDto(
                a.Id,
                a.PatientId,
                a.TreatmentRoomId,
                a.DentistId,
                a.PlanItemId,
                a.PlanItemId.HasValue && planItemLookup.TryGetValue(a.PlanItemId.Value, out var description)
                    ? description
                    : null,
                a.StartAt,
                a.EndAt,
                a.TypeName))
            .ToArray();
    }

    public async Task<Result<Guid>> CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanCreateOperationalData();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        if (request.EndAt <= request.StartAt)
        {
            return Result<Guid>.Failure("Appointment end time must be later than start time.");
        }
        if (string.IsNullOrWhiteSpace(request.TypeName))
        {
            return Result<Guid>.Failure("Appointment type is required.");
        }

        PlanItem? planItem = null;
        if (request.PlanItemId.HasValue)
        {
            planItem = await _planItems.GetAsync(_tenant.Current.CompanyId, request.PlanItemId.Value, ct);
            if (planItem is null)
            {
                return Result<Guid>.Failure("Selected treatment plan item was not found.");
            }
        }

        var item = new Appointment
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            TreatmentRoomId = request.TreatmentRoomId,
            DentistId = request.DentistId,
            PlanItemId = request.PlanItemId,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            TypeName = request.TypeName.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _appointments.AddAsync(item, ct);
        await _audit.AddAsync(new AuditLog
        {
            CompanyId = _tenant.Current.CompanyId,
            EntityName = "Appointment",
            EntityId = item.Id.ToString(),
            Action = "Create",
            NewValues = $"{item.StartAt:O}|{item.EndAt:O}|{item.TypeName}|PlanItem={(planItem?.Id.ToString() ?? "-")}",
            ChangedByUserId = _user.Current.UserId,
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = _user.Current.UserId
        }, ct);

        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanDeleteOperationalData(_user.Current.UserId);
        if (permission.IsFailure)
        {
            return permission;
        }

        var item = await _appointments.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("Appointment not found.");
        }

        await _appointments.SoftDeleteAsync(item, _user.Current.UserId, ct);
        return Result.Success();
    }

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
