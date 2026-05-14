using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class CompanyMembershipRepository : ICompanyMembershipRepository
{
    private readonly AppDbContext _db;

    public CompanyMembershipRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(CompanyMembership membership, CancellationToken ct = default)
    {
        await _db.CompanyMemberships.AddAsync(membership, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> IsMemberAsync(Guid companyId, string userId, CancellationToken ct = default)
        => _db.CompanyMemberships.AnyAsync(m => m.CompanyId == companyId && m.UserId == userId, ct);

    public Task<string?> GetRoleAsync(Guid companyId, string userId, CancellationToken ct = default)
        => _db.CompanyMemberships
            .Where(m => m.CompanyId == companyId && m.UserId == userId)
            .Select(m => m.Role)
            .SingleOrDefaultAsync(ct)!;
}
