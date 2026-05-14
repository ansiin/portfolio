using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class TreatmentRoom : CompanyEntityBase
{
    public string Name { get; set; } = string.Empty;
}
