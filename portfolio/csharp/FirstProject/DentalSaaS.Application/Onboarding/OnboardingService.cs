using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Onboarding;

public sealed class OnboardingService : IOnboardingService
{
    private readonly ICompanyRepository _companies;
    private readonly ICompanyMembershipRepository _memberships;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IIdentityAccountService _accounts;

    public OnboardingService(
        ICompanyRepository companies,
        ICompanyMembershipRepository memberships,
        ISubscriptionRepository subscriptions,
        IIdentityAccountService accounts)
    {
        _companies = companies;
        _memberships = memberships;
        _subscriptions = subscriptions;
        _accounts = accounts;
    }

    public async Task<Result<OnboardingResult>> RegisterCompanyAsync(OnboardingRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.TenantSlug))
        {
            return Result<OnboardingResult>.Failure("Company name and slug are required.");
        }

        var slug = request.TenantSlug.Trim().ToLowerInvariant();
        if (await _companies.SlugExistsAsync(slug, ct))
        {
            return Result<OnboardingResult>.Failure("Tenant slug already exists.");
        }

        var company = new Company
        {
            Name = request.CompanyName.Trim(),
            Slug = slug,
            Tier = SubscriptionTier.Free,
            Settings = new DentalSaaS.Domain.Entities.CompanySettings
            {
                CompanyId = Guid.Empty,
                CountryCode = "US",
                XrayIntervalDays = 180,
                CreatedBy = "system"
            }
        };

        await _companies.AddAsync(company, ct);
        company.Settings!.CompanyId = company.Id;

        var ownerUser = await _accounts.CreateUserAsync(
            request.OwnerEmail,
            request.OwnerDisplayName,
            request.OwnerPassword,
            ct: ct);
        if (ownerUser.IsFailure || string.IsNullOrWhiteSpace(ownerUser.Value))
        {
            return Result<OnboardingResult>.Failure(ownerUser.Error ?? "Owner account creation failed.");
        }

        await _memberships.AddAsync(new CompanyMembership
        {
            CompanyId = company.Id,
            UserId = ownerUser.Value,
            Role = Roles.CompanyOwner
        }, ct);

        await _subscriptions.UpsertAsync(new Subscription
        {
            CompanyId = company.Id,
            Tier = SubscriptionTier.Free,
            CreatedBy = "system"
        }, ct);

        return Result<OnboardingResult>.Success(new OnboardingResult(company.Id, ownerUser.Value));
    }
}
