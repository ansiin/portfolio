using DentalSaaS.Domain.Entities;

namespace DentalSaaS.Application.Abstractions;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveAsync(Guid companyId, CancellationToken ct = default);
    Task UpsertAsync(Subscription subscription, CancellationToken ct = default);
}
