using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class Treatment : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public Guid TreatmentTypeId { get; set; }
    public DateTimeOffset PerformedAt { get; set; }
    public decimal Cost { get; set; }
}
