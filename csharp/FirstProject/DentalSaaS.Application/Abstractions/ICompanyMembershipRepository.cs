using DentalSaaS.Domain.Entities;

namespace DentalSaaS.Application.Abstractions;

public interface ICompanyMembershipRepository
{
    Task AddAsync(CompanyMembership membership, CancellationToken ct = default);
    Task<bool> IsMemberAsync(Guid companyId, string userId, CancellationToken ct = default);
    Task<string?> GetRoleAsync(Guid companyId, string userId, CancellationToken ct = default);
}
