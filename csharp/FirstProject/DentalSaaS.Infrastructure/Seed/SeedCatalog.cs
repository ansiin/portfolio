using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Infrastructure.Seed;

public static class SeedCatalog
{
    public static readonly string[] SeedRoles =
    [
        Roles.SystemAdmin,
        Roles.SystemSupport,
        Roles.SystemBilling,
        Roles.CompanyOwner,
        Roles.CompanyAdmin,
        Roles.CompanyManager,
        Roles.CompanyEmployee
    ];

    public static readonly SubscriptionTier[] SeedSubscriptionTiers =
    [
        SubscriptionTier.Free,
        SubscriptionTier.Standard,
        SubscriptionTier.Premium
    ];
}
