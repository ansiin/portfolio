using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Patients;

public sealed class PatientService : IPatientService
{
    private readonly IPatientRepository _patients;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IAuditLogRepository _audit;
    private readonly IRoleAuthorizationService _authorization;

    public PatientService(
        IPatientRepository patients,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IAuditLogRepository audit,
        IRoleAuthorizationService authorization)
    {
        _patients = patients;
        _tenant = tenant;
        _user = user;
        _audit = audit;
        _authorization = authorization;
    }

    public async Task<IReadOnlyCollection<PatientListItem>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanViewOperationalData();
        if (permission.IsFailure)
        {
            return [];
        }

        var entities = await _patients.ListByCompanyAsync(_tenant.Current.CompanyId, ct);
        return entities
            .Select(p => new PatientListItem(p.Id, $"{p.FirstName} {p.LastName}", p.DateOfBirth, p.Email))
            .OrderBy(p => p.FullName)
            .ToArray();
    }

    public async Task<PatientListItem?> GetAsync(Guid patientId, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanViewOperationalData();
        if (permission.IsFailure)
        {
            return null;
        }

        var entity = await _patients.GetAsync(_tenant.Current.CompanyId, patientId, ct);
        return entity is null
            ? null
            : new PatientListItem(entity.Id, $"{entity.FirstName} {entity.LastName}", entity.DateOfBirth, entity.Email);
    }

    public async Task<Result<Guid>> CreateAsync(CreatePatientRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanCreateOperationalData();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return Result<Guid>.Failure("First and last name are required.");
        }

        var entity = new Patient
        {
            CompanyId = _tenant.Current.CompanyId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Email = request.Email.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _patients.AddAsync(entity, ct);
        await WriteAuditAsync(entity.Id.ToString(), "Patient", "Create", null, $"{entity.FirstName} {entity.LastName}", ct);

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result> UpdateAsync(UpdatePatientRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var entity = await _patients.GetAsync(_tenant.Current.CompanyId, request.Id, ct);
        if (entity is null)
        {
            return Result.Failure("Patient not found.");
        }

        var permission = _authorization.EnsureCanEditOperationalData(entity.CreatedBy);
        if (permission.IsFailure)
        {
            return permission;
        }

        var oldValues = $"{entity.FirstName} {entity.LastName}|{entity.Email}";
        entity.FirstName = request.FirstName.Trim();
        entity.LastName = request.LastName.Trim();
        entity.DateOfBirth = request.DateOfBirth;
        entity.Email = request.Email.Trim();
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedBy = _user.Current.UserId;

        await _patients.UpdateAsync(entity, ct);
        await WriteAuditAsync(entity.Id.ToString(), "Patient", "Update", oldValues, $"{entity.FirstName} {entity.LastName}|{entity.Email}", ct);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid patientId, CancellationToken ct = default)
    {
        EnsureTenant();
        var entity = await _patients.GetAsync(_tenant.Current.CompanyId, patientId, ct);
        if (entity is null)
        {
            return Result.Failure("Patient not found.");
        }

        var permission = _authorization.EnsureCanDeleteOperationalData(entity.CreatedBy);
        if (permission.IsFailure)
        {
            return permission;
        }

        await _patients.SoftDeleteAsync(entity, _user.Current.UserId, ct);
        await WriteAuditAsync(entity.Id.ToString(), "Patient", "Delete", $"{entity.FirstName} {entity.LastName}", null, ct);

        return Result.Success();
    }

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }

    private Task WriteAuditAsync(
        string entityId,
        string entityName,
        string action,
        string? oldValues,
        string? newValues,
        CancellationToken ct)
    {
        return _audit.AddAsync(new AuditLog
        {
            CompanyId = _tenant.Current.CompanyId,
            EntityId = entityId,
            EntityName = entityName,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedByUserId = _user.Current.UserId,
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = _user.Current.UserId
        }, ct);
    }
}
