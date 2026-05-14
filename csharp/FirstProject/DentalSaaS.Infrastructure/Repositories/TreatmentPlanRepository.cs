using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class TreatmentPlanRepository : ITreatmentPlanRepository
{
    private readonly AppDbContext _db;

    public TreatmentPlanRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<TreatmentPlan?> GetWithItemsAsync(Guid companyId, Guid planId, CancellationToken ct = default)
        => _db.TreatmentPlans
            .Include(p => p.Items)
            .SingleOrDefaultAsync(p => p.CompanyId == companyId && p.Id == planId, ct);

    public async Task<IReadOnlyCollection<TreatmentPlan>> ListAsync(Guid companyId, CancellationToken ct = default)
    {
        var plans = await _db.TreatmentPlans
            .Include(p => p.Items)
            .Where(p => p.CompanyId == companyId)
            .ToListAsync(ct);

        return plans
            .OrderByDescending(p => p.CreatedAt)
            .ToArray();
    }

    public async Task AddAsync(TreatmentPlan plan, CancellationToken ct = default)
    {
        await _db.TreatmentPlans.AddAsync(plan, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddItemAsync(PlanItem item, CancellationToken ct = default)
    {
        await _db.PlanItems.AddAsync(item, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateItemAsync(PlanItem item, CancellationToken ct = default)
    {
        _db.PlanItems.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TreatmentPlan plan, CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
