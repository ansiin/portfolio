using DentalSaaS.Domain.Common;
using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Domain.Entities;

public sealed class Subscription : CompanyEntityBase
{
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;
    public DateTimeOffset ValidFrom { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ValidTo { get; set; }
}
