using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class Dentist : CompanyEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
}
