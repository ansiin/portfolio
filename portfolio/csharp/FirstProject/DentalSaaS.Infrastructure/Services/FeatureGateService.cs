using DentalSaaS.Application.Abstractions;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Infrastructure.Services;

public sealed class FeatureGateService : IFeatureGateService
{
    private readonly ISubscriptionRepository _subscriptions;

    public FeatureGateService(ISubscriptionRepository subscriptions)
    {
        _subscriptions = subscriptions;
    }

    public async Task<Result> EnsureAllowedAsync(Guid companyId, Feature feature, CancellationToken ct = default)
    {
        var subscription = await _subscriptions.GetActiveAsync(companyId, ct);
        var tier = subscription?.Tier ?? SubscriptionTier.Free;

        return feature switch
        {
            Feature.InsuranceModule when tier == SubscriptionTier.Free
                => Result.Failure("Insurance module requires Standard or Premium."),
            Feature.PaymentPlan when tier != SubscriptionTier.Premium
                => Result.Failure("Payment plans require Premium."),
            _ => Result.Success()
        };
    }
}
