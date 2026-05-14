using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _db;

    public CompanyRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Company?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => _db.Companies.Include(c => c.Settings).SingleOrDefaultAsync(c => c.Slug == slug, ct);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
        => _db.Companies.AnyAsync(c => c.Slug == slug, ct);

    public async Task AddAsync(Company company, CancellationToken ct = default)
    {
        await _db.Companies.AddAsync(company, ct);
        if (company.Settings is not null)
        {
            company.Settings.CompanyId = company.Id;
            await _db.CompanySettings.AddAsync(company.Settings, ct);
        }

        await _db.SaveChangesAsync(ct);
    }
}
