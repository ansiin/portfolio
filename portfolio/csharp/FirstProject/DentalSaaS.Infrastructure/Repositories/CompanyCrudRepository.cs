using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Common;
using DentalSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class CompanyCrudRepository<TEntity> : ICompanyCrudRepository<TEntity>
    where TEntity : CompanyEntityBase
{
    private readonly AppDbContext _db;

    public CompanyCrudRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<TEntity>> ListAsync(Guid companyId, CancellationToken ct = default)
    {
        var items = await _db.Set<TEntity>()
            .Where(x => x.CompanyId == companyId)
            .ToArrayAsync(ct);

        return items
            .OrderByDescending(x => x.CreatedAt)
            .ToArray();
    }

    public Task<TEntity?> GetAsync(Guid companyId, Guid id, CancellationToken ct = default)
    {
        return _db.Set<TEntity>()
            .SingleOrDefaultAsync(x => x.CompanyId == companyId && x.Id == id, ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _db.Set<TEntity>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _db.Set<TEntity>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(TEntity entity, string deletedBy, CancellationToken ct = default)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        entity.DeletedBy = deletedBy;
        _db.Set<TEntity>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }
}
