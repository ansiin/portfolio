using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Appointments;

public sealed class AppointmentFormViewModel
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid TreatmentRoomId { get; set; }

    [Required]
    public Guid DentistId { get; set; }

    public Guid? PlanItemId { get; set; }

    [Required]
    public DateTimeOffset StartAt { get; set; }

    [Required]
    public DateTimeOffset EndAt { get; set; }

    [Required]
    [StringLength(120)]
    public string TypeName { get; set; } = string.Empty;
}
