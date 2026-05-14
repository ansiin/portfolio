using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Onboarding;

public sealed record OnboardingRequest(string CompanyName, string TenantSlug, string OwnerEmail, string OwnerPassword, string OwnerDisplayName);
public sealed record OnboardingResult(Guid CompanyId, string OwnerUserId);

public interface IOnboardingService
{
    Task<Result<OnboardingResult>> RegisterCompanyAsync(OnboardingRequest request, CancellationToken ct = default);
}
