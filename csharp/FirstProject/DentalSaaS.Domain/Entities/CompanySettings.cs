using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class CompanySettings : CompanyEntityBase
{
    public int XrayIntervalDays { get; set; } = 180;
    public string CountryCode { get; set; } = "US";
}
