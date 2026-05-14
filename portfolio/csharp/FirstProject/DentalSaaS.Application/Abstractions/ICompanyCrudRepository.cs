using DentalSaaS.Domain.Common;

namespace DentalSaaS.Application.Abstractions;

public interface ICompanyCrudRepository<TEntity>
    where TEntity : CompanyEntityBase
{
    Task<IReadOnlyCollection<TEntity>> ListAsync(Guid companyId, CancellationToken ct = default);
    Task<TEntity?> GetAsync(Guid companyId, Guid id, CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task SoftDeleteAsync(TEntity entity, string deletedBy, CancellationToken ct = default);
}
