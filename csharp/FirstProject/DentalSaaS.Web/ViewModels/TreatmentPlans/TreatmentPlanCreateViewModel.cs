using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.TreatmentPlans;

public sealed class TreatmentPlanCreateViewModel
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;
}
