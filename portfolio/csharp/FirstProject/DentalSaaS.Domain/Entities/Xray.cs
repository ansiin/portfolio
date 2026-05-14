using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class Xray : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public DateTimeOffset TakenAt { get; set; }
    public string FileUrl { get; set; } = string.Empty;
}
