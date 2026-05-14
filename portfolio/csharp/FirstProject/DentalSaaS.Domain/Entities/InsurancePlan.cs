using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class InsurancePlan : CompanyEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string CoverageType { get; set; } = string.Empty;
}
