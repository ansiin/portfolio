using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class TreatmentType : CompanyEntityBase
{
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}
