using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.TreatmentPlans;

public sealed class AddPlanItemViewModel
{
    [Required]
    public Guid PlanId { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.0, 999999.0)]
    public decimal EstimatedCost { get; set; }

    [Range(1, 100)]
    public int Sequence { get; set; } = 1;

    [Range(1, 5)]
    public int Urgency { get; set; } = 3;
}
