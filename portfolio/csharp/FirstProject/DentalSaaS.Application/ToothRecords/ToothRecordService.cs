using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.ToothRecords;

public sealed class ToothRecordService : IToothRecordService
{
    private readonly ICompanyCrudRepository<ToothRecord> _records;
    private readonly IPatientRepository _patients;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;
    private readonly IAuditLogRepository _audit;

    public ToothRecordService(
        ICompanyCrudRepository<ToothRecord> records,
        IPatientRepository patients,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization,
        IAuditLogRepository audit)
    {
        _records = records;
        _patients = patients;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
        _audit = audit;
    }

    public async Task<IReadOnlyCollection<ToothRecordDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanOperateBasicRecords().IsFailure)
        {
            return [];
        }

        var items = await _records.ListAsync(_tenant.Current.CompanyId, ct);
        return items
            .OrderBy(x => x.PatientId)
            .ThenBy(x => x.ToothNumber)
            .Select(Map)
            .ToArray();
    }

    public async Task<ToothRecordDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanOperateBasicRecords().IsFailure)
        {
            return null;
        }

        var item = await _records.GetAsync(_tenant.Current.CompanyId, id, ct);
        return item is null ? null : Map(item);
    }

    public async Task<Result<Guid>> CreateAsync(CreateToothRecordRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanOperateBasicRecords();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        var validation = Validate(request.ToothNumber, request.ConditionStatus, request.PatientId);
        if (validation.IsFailure)
        {
            return Result<Guid>.Failure(validation.Error ?? "Validation failed.");
        }

        var patient = await _patients.GetAsync(_tenant.Current.CompanyId, request.PatientId, ct);
        if (patient is null)
        {
            return Result<Guid>.Failure("Selected patient was not found.");
        }

        var existingForTooth = (await _records.ListAsync(_tenant.Current.CompanyId, ct))
            .Any(x => x.PatientId == request.PatientId && x.ToothNumber == request.ToothNumber);
        if (existingForTooth)
        {
            return Result<Guid>.Failure("A record for this patient and tooth number already exists.");
        }

        var item = new ToothRecord
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            ToothNumber = request.ToothNumber,
            ConditionStatus = request.ConditionStatus.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _records.AddAsync(item, ct);
        await WriteAuditAsync(item.Id, "Create", null, $"{item.PatientId}|{item.ToothNumber}|{item.ConditionStatus}", ct);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> UpdateAsync(UpdateToothRecordRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var item = await _records.GetAsync(_tenant.Current.CompanyId, request.Id, ct);
        if (item is null)
        {
            return Result.Failure("Tooth record not found.");
        }

        var permission = _authorization.EnsureCanOperateBasicRecords();
        if (permission.IsFailure)
        {
            return permission;
        }

        var validation = Validate(request.ToothNumber, request.ConditionStatus, item.PatientId);
        if (validation.IsFailure)
        {
            return validation;
        }

        var conflict = (await _records.ListAsync(_tenant.Current.CompanyId, ct))
            .Any(x => x.Id != item.Id && x.PatientId == item.PatientId && x.ToothNumber == request.ToothNumber);
        if (conflict)
        {
            return Result.Failure("A record for this patient and tooth number already exists.");
        }

        var old = $"{item.PatientId}|{item.ToothNumber}|{item.ConditionStatus}|{item.Notes}";
        item.ToothNumber = request.ToothNumber;
        item.ConditionStatus = request.ConditionStatus.Trim();
        item.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        item.UpdatedBy = _user.Current.UserId;

        await _records.UpdateAsync(item, ct);
        await WriteAuditAsync(item.Id, "Update", old, $"{item.PatientId}|{item.ToothNumber}|{item.ConditionStatus}|{item.Notes}", ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var item = await _records.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("Tooth record not found.");
        }

        var permission = _authorization.EnsureCanDeleteOperationalData(item.CreatedBy);
        if (permission.IsFailure)
        {
            return permission;
        }

        await _records.SoftDeleteAsync(item, _user.Current.UserId, ct);
        await WriteAuditAsync(item.Id, "Delete", $"{item.PatientId}|{item.ToothNumber}|{item.ConditionStatus}", null, ct);
        return Result.Success();
    }

    private static Result Validate(int toothNumber, string conditionStatus, Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            return Result.Failure("Patient is required.");
        }
        if (toothNumber is < 1 or > 32)
        {
            return Result.Failure("Tooth number must be between 1 and 32.");
        }
        if (string.IsNullOrWhiteSpace(conditionStatus))
        {
            return Result.Failure("Condition status is required.");
        }

        return Result.Success();
    }

    private ToothRecordDto Map(ToothRecord entity)
        => new(entity.Id, entity.PatientId, entity.ToothNumber, entity.ConditionStatus, entity.Notes, entity.CreatedAt);

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
            EntityName = "ToothRecord",
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedByUserId = _user.Current.UserId,
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = _user.Current.UserId
        }, ct);
    }
}
