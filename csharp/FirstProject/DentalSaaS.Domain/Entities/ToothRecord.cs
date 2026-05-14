using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class ToothRecord : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public int ToothNumber { get; set; }
    public string ConditionStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
