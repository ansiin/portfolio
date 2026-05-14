using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Insurance;

public interface IInsuranceService
{
    Task<IReadOnlyCollection<InsurancePlanDto>> ListPlansAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<CostEstimateDto>> ListEstimatesAsync(CancellationToken ct = default);
    Task<Result<Guid>> CreatePlanAsync(CreateInsurancePlanRequest request, CancellationToken ct = default);
    Task<Result<Guid>> CreateEstimateAsync(CreateCostEstimateRequest request, CancellationToken ct = default);
    Task<Result> UpdateClaimStatusAsync(Guid estimateId, string claimStatus, CancellationToken ct = default);
    Task<Result> SubmitEstimateAsync(Guid estimateId, CancellationToken ct = default);
    Task<Result> SetSubmissionStateAsync(Guid estimateId, DentalSaaS.Shared.Enums.InsuranceSubmissionState state, string? message, CancellationToken ct = default);
    Task<Result> DeletePlanAsync(Guid id, CancellationToken ct = default);
    Task<Result> DeleteEstimateAsync(Guid id, CancellationToken ct = default);
}
