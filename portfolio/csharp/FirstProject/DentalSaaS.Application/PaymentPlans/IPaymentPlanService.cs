using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.PaymentPlans;

public interface IPaymentPlanService
{
    Task<IReadOnlyCollection<PaymentPlanDto>> ListAsync(CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreatePaymentPlanRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
