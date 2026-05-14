using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.TreatmentPlans;

public sealed class TreatmentPlanService : ITreatmentPlanService
{
    private readonly ITreatmentPlanRepository _plans;
    private readonly ICompanyCrudRepository<Appointment> _appointments;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IAuditLogRepository _audit;
    private readonly IRoleAuthorizationService _authorization;

    public TreatmentPlanService(
        ITreatmentPlanRepository plans,
        ICompanyCrudRepository<Appointment> appointments,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IAuditLogRepository audit,
        IRoleAuthorizationService authorization)
    {
        _plans = plans;
        _appointments = appointments;
        _tenant = tenant;
        _user = user;
        _audit = audit;
        _authorization = authorization;
    }

    public async Task<IReadOnlyCollection<TreatmentPlanDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanViewOperationalData();
        if (permission.IsFailure)
        {
            return [];
        }

        var plans = await _plans.ListAsync(_tenant.Current.CompanyId, ct);
        var appointmentLookup = await BuildAppointmentLookupAsync(ct);
        return plans.Select(p => Map(p, appointmentLookup)).ToArray();
    }

    public async Task<TreatmentPlanDto?> GetAsync(Guid planId, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanViewOperationalData();
        if (permission.IsFailure)
        {
            return null;
        }

        var plan = await _plans.GetWithItemsAsync(_tenant.Current.CompanyId, planId, ct);
        if (plan is null)
        {
            return null;
        }

        var appointmentLookup = await BuildAppointmentLookupAsync(ct);
        return Map(plan, appointmentLookup);
    }

    public async Task<Result<Guid>> CreatePlanAsync(CreateTreatmentPlanRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageClinicalPlans();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<Guid>.Failure("Plan title is required.");
        }

        var plan = new TreatmentPlan
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            Title = request.Title.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _plans.AddAsync(plan, ct);
        await WriteAuditAsync(plan.Id, "Create", null, plan.Title, ct);
        return Result<Guid>.Success(plan.Id);
    }

    public async Task<Result<Guid>> AddItemAsync(AddPlanItemRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var plan = await _plans.GetWithItemsAsync(_tenant.Current.CompanyId, request.PlanId, ct);
        if (plan is null)
        {
            return Result<Guid>.Failure("Plan not found.");
        }

        var permission = _authorization.EnsureCanManageClinicalPlans();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        var item = new PlanItem
        {
            CompanyId = _tenant.Current.CompanyId,
            TreatmentPlanId = plan.Id,
            Description = request.Description.Trim(),
            EstimatedCost = request.EstimatedCost,
            Sequence = request.Sequence,
            Urgency = request.Urgency,
            CreatedBy = _user.Current.UserId
        };

        await _plans.AddItemAsync(item, ct);
        await WriteAuditAsync(plan.Id, "AddItem", null, item.Description, ct);

        return Result<Guid>.Success(item.Id);
    }

    public Task<Result> AcceptItemAsync(Guid planId, Guid itemId, CancellationToken ct = default)
        => DecideItemAsync(planId, itemId, PlanItemDecisionStatus.Accepted, ct);

    public Task<Result> DeferItemAsync(Guid planId, Guid itemId, CancellationToken ct = default)
        => DecideItemAsync(planId, itemId, PlanItemDecisionStatus.Deferred, ct);

    private async Task<Result> DecideItemAsync(Guid planId, Guid itemId, PlanItemDecisionStatus status, CancellationToken ct)
    {
        EnsureTenant();
        var plan = await _plans.GetWithItemsAsync(_tenant.Current.CompanyId, planId, ct);
        if (plan is null)
        {
            return Result.Failure("Plan not found.");
        }

        var permission = _authorization.EnsureCanManageClinicalPlans();
        if (permission.IsFailure)
        {
            return permission;
        }

        var item = plan.Items.SingleOrDefault(i => i.Id == itemId && !i.IsDeleted);
        if (item is null)
        {
            return Result.Failure("Plan item not found.");
        }

        var old = item.DecisionStatus.ToString();
        item.DecisionStatus = status;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        item.UpdatedBy = _user.Current.UserId;

        await _plans.UpdateItemAsync(item, ct);
        await WriteAuditAsync(plan.Id, "PlanItemDecision", old, status.ToString(), ct);

        return Result.Success();
    }

    private async Task<IReadOnlyDictionary<Guid, (int Count, DateTimeOffset? NextAt)>> BuildAppointmentLookupAsync(CancellationToken ct)
    {
        var appointments = await _appointments.ListAsync(_tenant.Current.CompanyId, ct);
        return appointments
            .Where(a => a.PlanItemId.HasValue)
            .GroupBy(a => a.PlanItemId!.Value)
            .ToDictionary(
                g => g.Key,
                g => (g.Count(), g.OrderBy(a => a.StartAt).Select(a => (DateTimeOffset?)a.StartAt).FirstOrDefault()));
    }

    private TreatmentPlanDto Map(TreatmentPlan plan, IReadOnlyDictionary<Guid, (int Count, DateTimeOffset? NextAt)> appointmentLookup)
        => new(
            plan.Id,
            plan.PatientId,
            plan.Title,
            plan.Items
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.Sequence)
                .Select(i =>
                {
                    var hasAppointments = appointmentLookup.TryGetValue(i.Id, out var value);
                    return new PlanItemDto(
                        i.Id,
                        i.Description,
                        i.EstimatedCost,
                        i.Sequence,
                        i.Urgency,
                        i.DecisionStatus,
                        hasAppointments ? value.Count : 0,
                        hasAppointments ? value.NextAt : null);
                })
                .ToArray());

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }

    private Task WriteAuditAsync(Guid entityId, string action, string? oldValues, string? newValues, CancellationToken ct)
    {
        return _audit.AddAsync(new AuditLog
        {
            CompanyId = _tenant.Current.CompanyId,
            EntityId = entityId.ToString(),
            EntityName = "TreatmentPlan",
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedByUserId = _user.Current.UserId,
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = _user.Current.UserId
        }, ct);
    }
}
