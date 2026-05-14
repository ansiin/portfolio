using DentalSaaS.Domain.Common;
using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Domain.Entities;

public sealed class Company : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Free;
    public bool IsActive { get; set; } = true;
    public CompanySettings? Settings { get; set; }
}
