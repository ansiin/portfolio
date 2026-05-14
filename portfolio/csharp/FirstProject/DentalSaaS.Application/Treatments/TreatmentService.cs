using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Treatments;

public sealed class TreatmentService : ITreatmentService
{
    private readonly ICompanyCrudRepository<Treatment> _treatments;
    private readonly ICompanyCrudRepository<TreatmentType> _types;
    private readonly IPatientRepository _patients;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;

    public TreatmentService(
        ICompanyCrudRepository<Treatment> treatments,
        ICompanyCrudRepository<TreatmentType> types,
        IPatientRepository patients,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization)
    {
        _treatments = treatments;
        _types = types;
        _patients = patients;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
    }

    public async Task<IReadOnlyCollection<TreatmentDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanOperateBasicRecords().IsFailure)
        {
            return [];
        }

        var items = await _treatments.ListAsync(_tenant.Current.CompanyId, ct);
        return items
            .OrderByDescending(t => t.PerformedAt)
            .Select(Map)
            .ToArray();
    }

    public async Task<TreatmentDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanOperateBasicRecords().IsFailure)
        {
            return null;
        }

        var item = await _treatments.GetAsync(_tenant.Current.CompanyId, id, ct);
        return item is null ? null : Map(item);
    }

    public async Task<Result<Guid>> CreateAsync(CreateTreatmentRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanOperateBasicRecords();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        var validation = await ValidateAsync(request.PatientId, request.TreatmentTypeId, request.Cost, ct);
        if (validation.IsFailure)
        {
            return Result<Guid>.Failure(validation.Error ?? "Validation failed.");
        }

        var item = new Treatment
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            TreatmentTypeId = request.TreatmentTypeId,
            PerformedAt = request.PerformedAt,
            Cost = request.Cost,
            CreatedBy = _user.Current.UserId
        };

        await _treatments.AddAsync(item, ct);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> UpdateAsync(UpdateTreatmentRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var item = await _treatments.GetAsync(_tenant.Current.CompanyId, request.Id, ct);
        if (item is null)
        {
            return Result.Failure("Treatment not found.");
        }

        var permission = _authorization.EnsureCanOperateBasicRecords();
        if (permission.IsFailure)
        {
            return permission;
        }

        var validation = await ValidateAsync(item.PatientId, request.TreatmentTypeId, request.Cost, ct);
        if (validation.IsFailure)
        {
            return validation;
        }

        item.TreatmentTypeId = request.TreatmentTypeId;
        item.PerformedAt = request.PerformedAt;
        item.Cost = request.Cost;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        item.UpdatedBy = _user.Current.UserId;

        await _treatments.UpdateAsync(item, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var item = await _treatments.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("Treatment not found.");
        }

        var permission = _authorization.EnsureCanDeleteOperationalData(item.CreatedBy);
        if (permission.IsFailure)
        {
            return permission;
        }

        await _treatments.SoftDeleteAsync(item, _user.Current.UserId, ct);
        return Result.Success();
    }

    private async Task<Result> ValidateAsync(Guid patientId, Guid treatmentTypeId, decimal cost, CancellationToken ct)
    {
        if (patientId == Guid.Empty)
        {
            return Result.Failure("Patient is required.");
        }
        if (treatmentTypeId == Guid.Empty)
        {
            return Result.Failure("Treatment type is required.");
        }
        if (cost <= 0)
        {
            return Result.Failure("Cost must be greater than 0.");
        }

        var patient = await _patients.GetAsync(_tenant.Current.CompanyId, patientId, ct);
        if (patient is null)
        {
            return Result.Failure("Selected patient was not found.");
        }

        var type = await _types.GetAsync(_tenant.Current.CompanyId, treatmentTypeId, ct);
        if (type is null)
        {
            return Result.Failure("Selected treatment type was not found.");
        }

        return Result.Success();
    }

    private static TreatmentDto Map(Treatment entity)
        => new(entity.Id, entity.PatientId, entity.TreatmentTypeId, entity.PerformedAt, entity.Cost);

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
