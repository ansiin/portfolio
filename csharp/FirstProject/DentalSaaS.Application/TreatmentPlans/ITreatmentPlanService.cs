using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.TreatmentPlans;

public interface ITreatmentPlanService
{
    Task<IReadOnlyCollection<TreatmentPlanDto>> ListAsync(CancellationToken ct = default);
    Task<TreatmentPlanDto?> GetAsync(Guid planId, CancellationToken ct = default);
    Task<Result<Guid>> CreatePlanAsync(CreateTreatmentPlanRequest request, CancellationToken ct = default);
    Task<Result<Guid>> AddItemAsync(AddPlanItemRequest request, CancellationToken ct = default);
    Task<Result> AcceptItemAsync(Guid planId, Guid itemId, CancellationToken ct = default);
    Task<Result> DeferItemAsync(Guid planId, Guid itemId, CancellationToken ct = default);
}
