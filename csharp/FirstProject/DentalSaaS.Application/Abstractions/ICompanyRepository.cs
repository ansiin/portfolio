using DentalSaaS.Domain.Entities;

namespace DentalSaaS.Application.Abstractions;

public interface ICompanyRepository
{
    Task<Company?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
    Task AddAsync(Company company, CancellationToken ct = default);
}
