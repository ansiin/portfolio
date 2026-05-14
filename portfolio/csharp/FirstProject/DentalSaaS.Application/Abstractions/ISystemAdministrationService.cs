using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Abstractions;

public sealed record SystemCompanyItem(Guid CompanyId, string Name, string Slug, SubscriptionTier Tier, bool IsActive);
public sealed record SystemSubscriptionItem(Guid CompanyId, string CompanySlug, SubscriptionTier Tier, DateTimeOffset ValidFrom);

public interface ISystemAdministrationService
{
    Task<IReadOnlyCollection<SystemCompanyItem>> ListCompaniesAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<SystemSubscriptionItem>> ListSubscriptionsAsync(CancellationToken ct = default);
    Task<Result> SetCompanyActiveAsync(Guid companyId, bool isActive, string changedByUserId, CancellationToken ct = default);
    Task<Result> ChangeSubscriptionTierAsync(Guid companyId, SubscriptionTier tier, string changedByUserId, CancellationToken ct = default);
    Task<Result<Guid>> BeginImpersonationAsync(string adminUserId, string targetUserId, Guid companyId, string reason, CancellationToken ct = default);
    Task EndImpersonationAsync(Guid sessionId, CancellationToken ct = default);
}
