using DentalSaaS.Domain.Entities;

namespace DentalSaaS.Application.Abstractions;

public interface ITreatmentPlanRepository
{
    Task<TreatmentPlan?> GetWithItemsAsync(Guid companyId, Guid planId, CancellationToken ct = default);
    Task<IReadOnlyCollection<TreatmentPlan>> ListAsync(Guid companyId, CancellationToken ct = default);
    Task AddAsync(TreatmentPlan plan, CancellationToken ct = default);
    Task AddItemAsync(PlanItem item, CancellationToken ct = default);
    Task UpdateItemAsync(PlanItem item, CancellationToken ct = default);
    Task UpdateAsync(TreatmentPlan plan, CancellationToken ct = default);
}
