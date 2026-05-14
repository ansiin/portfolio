using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Insurance;

public sealed class CostEstimateFormViewModel
{
    [Required]
    public Guid PatientId { get; set; }

    public Guid? InsurancePlanId { get; set; }

    [Required]
    [StringLength(40)]
    public string CountryTemplate { get; set; } = "US-DEFAULT";

    [Required]
    [Range(0.01, 1000000)]
    public decimal TotalAmount { get; set; }
}
