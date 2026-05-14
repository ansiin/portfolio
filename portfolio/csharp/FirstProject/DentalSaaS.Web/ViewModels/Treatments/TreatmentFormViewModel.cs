using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Treatments;

public sealed class TreatmentFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid TreatmentTypeId { get; set; }

    [Required]
    public DateTimeOffset PerformedAt { get; set; } = DateTimeOffset.UtcNow;

    [Range(0.01, 1000000)]
    public decimal Cost { get; set; }
}
