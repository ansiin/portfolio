using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _db;

    public SubscriptionRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Subscription?> GetActiveAsync(Guid companyId, CancellationToken ct = default)
        => GetActiveInternalAsync(companyId, ct);

    public async Task UpsertAsync(Subscription subscription, CancellationToken ct = default)
    {
        var existing = await GetActiveInternalAsync(subscription.CompanyId, ct);

        if (existing is not null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTimeOffset.UtcNow;
            existing.DeletedBy = "system";
            _db.Subscriptions.Update(existing);
        }

        await _db.Subscriptions.AddAsync(subscription, ct);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Subscription?> GetActiveInternalAsync(Guid companyId, CancellationToken ct)
    {
        var items = await _db.Subscriptions
            .Where(s => s.CompanyId == companyId)
            .ToListAsync(ct);

        return items
            .OrderByDescending(s => s.ValidFrom)
            .FirstOrDefault();
    }
}
