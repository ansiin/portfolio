using System.Text.Json;
using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Insurance;

public sealed class InsuranceService : IInsuranceService
{
    private readonly ICompanyCrudRepository<InsurancePlan> _plans;
    private readonly ICompanyCrudRepository<CostEstimate> _estimates;
    private readonly ICompanyCrudRepository<Patient> _patients;
    private readonly IInsuranceSubmissionGateway _submissionGateway;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;
    private readonly IFeatureGateService _features;

    public InsuranceService(
        ICompanyCrudRepository<InsurancePlan> plans,
        ICompanyCrudRepository<CostEstimate> estimates,
        ICompanyCrudRepository<Patient> patients,
        IInsuranceSubmissionGateway submissionGateway,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization,
        IFeatureGateService features)
    {
        _plans = plans;
        _estimates = estimates;
        _patients = patients;
        _submissionGateway = submissionGateway;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
        _features = features;
    }

    public async Task<IReadOnlyCollection<InsurancePlanDto>> ListPlansAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure || _authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var plans = await _plans.ListAsync(_tenant.Current.CompanyId, ct);
        return plans
            .OrderBy(p => p.Name)
            .Select(p => new InsurancePlanDto(p.Id, p.Name, p.CoverageType))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<CostEstimateDto>> ListEstimatesAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure || _authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var estimates = await _estimates.ListAsync(_tenant.Current.CompanyId, ct);
        return estimates
            .OrderByDescending(e => e.CreatedAt)
            .Select(Map)
            .ToArray();
    }

    public async Task<Result<Guid>> CreatePlanAsync(CreateInsurancePlanRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return Result<Guid>.Failure(feature.Error ?? "Insurance module is unavailable.");
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<Guid>.Failure("Insurance plan name is required.");
        }
        if (string.IsNullOrWhiteSpace(request.CoverageType))
        {
            return Result<Guid>.Failure("Coverage type is required.");
        }

        var plan = new InsurancePlan
        {
            CompanyId = _tenant.Current.CompanyId,
            Name = request.Name.Trim(),
            CoverageType = request.CoverageType.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _plans.AddAsync(plan, ct);
        return Result<Guid>.Success(plan.Id);
    }

    public async Task<Result<Guid>> CreateEstimateAsync(CreateCostEstimateRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return Result<Guid>.Failure(feature.Error ?? "Insurance module is unavailable.");
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }
        if (request.PatientId == Guid.Empty)
        {
            return Result<Guid>.Failure("Patient is required.");
        }
        if (request.TotalAmount <= 0)
        {
            return Result<Guid>.Failure("Total amount must be greater than 0.");
        }

        var patient = await _patients.GetAsync(_tenant.Current.CompanyId, request.PatientId, ct);
        if (patient is null)
        {
            return Result<Guid>.Failure("Selected patient was not found.");
        }

        var estimate = new CostEstimate
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            InsurancePlanId = request.InsurancePlanId,
            CountryTemplate = string.IsNullOrWhiteSpace(request.CountryTemplate) ? "US-DEFAULT" : request.CountryTemplate.Trim(),
            TotalAmount = request.TotalAmount,
            ClaimStatus = "Draft",
            SubmissionState = InsuranceSubmissionState.Draft,
            ProviderName = "MockInsuranceGateway",
            CreatedBy = _user.Current.UserId
        };

        await _estimates.AddAsync(estimate, ct);
        return Result<Guid>.Success(estimate.Id);
    }

    public async Task<Result> UpdateClaimStatusAsync(Guid estimateId, string claimStatus, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return feature;
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return permission;
        }

        var estimate = await _estimates.GetAsync(_tenant.Current.CompanyId, estimateId, ct);
        if (estimate is null)
        {
            return Result.Failure("Cost estimate not found.");
        }
        if (string.IsNullOrWhiteSpace(claimStatus))
        {
            return Result.Failure("Claim status is required.");
        }

        estimate.ClaimStatus = claimStatus.Trim();
        estimate.UpdatedAt = DateTimeOffset.UtcNow;
        estimate.UpdatedBy = _user.Current.UserId;

        await _estimates.UpdateAsync(estimate, ct);
        return Result.Success();
    }

    public async Task<Result> SubmitEstimateAsync(Guid estimateId, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return feature;
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return permission;
        }

        var estimate = await _estimates.GetAsync(_tenant.Current.CompanyId, estimateId, ct);
        if (estimate is null)
        {
            return Result.Failure("Cost estimate not found.");
        }

        var patient = await _patients.GetAsync(_tenant.Current.CompanyId, estimate.PatientId, ct);
        if (patient is null)
        {
            return Result.Failure("Patient not found for this estimate.");
        }

        var payloadObject = new
        {
            estimate.Id,
            estimate.CompanyId,
            estimate.PatientId,
            PatientName = $"{patient.FirstName} {patient.LastName}",
            estimate.CountryTemplate,
            estimate.TotalAmount,
            estimate.ClaimStatus,
            SubmittedBy = _user.Current.UserId,
            SubmittedAt = DateTimeOffset.UtcNow
        };

        var payload = JsonSerializer.Serialize(payloadObject);
        estimate.SubmissionState = InsuranceSubmissionState.Queued;
        estimate.SubmissionPayloadJson = payload;
        estimate.ProviderName = string.IsNullOrWhiteSpace(estimate.ProviderName) ? "MockInsuranceGateway" : estimate.ProviderName;
        estimate.UpdatedAt = DateTimeOffset.UtcNow;
        estimate.UpdatedBy = _user.Current.UserId;
        await _estimates.UpdateAsync(estimate, ct);

        var submission = await _submissionGateway.SubmitAsync(new InsuranceSubmissionRequest(
            estimate.Id,
            estimate.CompanyId,
            estimate.PatientId,
            estimate.CountryTemplate,
            estimate.TotalAmount,
            estimate.ProviderName,
            payload), ct);

        estimate.SubmissionState = submission.IsSuccess ? submission.SubmissionState : InsuranceSubmissionState.Error;
        estimate.SubmittedAt = DateTimeOffset.UtcNow;
        estimate.ExternalSubmissionId = submission.ExternalSubmissionId;
        estimate.ProviderResponseMessage = submission.Message;
        estimate.ClaimStatus = submission.SubmissionState switch
        {
            InsuranceSubmissionState.Accepted => "Approved",
            InsuranceSubmissionState.Rejected => "Rejected",
            InsuranceSubmissionState.Submitted => "Submitted",
            _ => estimate.ClaimStatus
        };
        estimate.UpdatedAt = DateTimeOffset.UtcNow;
        estimate.UpdatedBy = _user.Current.UserId;

        await _estimates.UpdateAsync(estimate, ct);
        return submission.IsSuccess
            ? Result.Success()
            : Result.Failure(submission.Message ?? "Submission failed.");
    }

    public async Task<Result> SetSubmissionStateAsync(Guid estimateId, InsuranceSubmissionState state, string? message, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return feature;
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return permission;
        }

        var estimate = await _estimates.GetAsync(_tenant.Current.CompanyId, estimateId, ct);
        if (estimate is null)
        {
            return Result.Failure("Cost estimate not found.");
        }

        estimate.SubmissionState = state;
        estimate.ProviderResponseMessage = string.IsNullOrWhiteSpace(message) ? estimate.ProviderResponseMessage : message.Trim();
        estimate.ClaimStatus = state switch
        {
            InsuranceSubmissionState.Accepted => "Approved",
            InsuranceSubmissionState.Rejected => "Rejected",
            InsuranceSubmissionState.Submitted => "Submitted",
            _ => estimate.ClaimStatus
        };
        estimate.UpdatedAt = DateTimeOffset.UtcNow;
        estimate.UpdatedBy = _user.Current.UserId;

        await _estimates.UpdateAsync(estimate, ct);
        return Result.Success();
    }

    public async Task<Result> DeletePlanAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return feature;
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return permission;
        }

        var plan = await _plans.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (plan is null)
        {
            return Result.Failure("Insurance plan not found.");
        }

        await _plans.SoftDeleteAsync(plan, _user.Current.UserId, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteEstimateAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var feature = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.InsuranceModule, ct);
        if (feature.IsFailure)
        {
            return feature;
        }

        var permission = _authorization.EnsureCanManageInsuranceRelationships();
        if (permission.IsFailure)
        {
            return permission;
        }

        var estimate = await _estimates.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (estimate is null)
        {
            return Result.Failure("Cost estimate not found.");
        }

        await _estimates.SoftDeleteAsync(estimate, _user.Current.UserId, ct);
        return Result.Success();
    }

    private static CostEstimateDto Map(CostEstimate estimate)
        => new(
            estimate.Id,
            estimate.PatientId,
            estimate.InsurancePlanId,
            estimate.CountryTemplate,
            estimate.TotalAmount,
            estimate.ClaimStatus,
            estimate.SubmissionState,
            estimate.SubmittedAt,
            estimate.ExternalSubmissionId,
            estimate.ProviderName,
            estimate.ProviderResponseMessage);

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
