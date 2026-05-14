using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class Appointment : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public Guid TreatmentRoomId { get; set; }
    public Guid DentistId { get; set; }
    public Guid? PlanItemId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string TypeName { get; set; } = string.Empty;
}
