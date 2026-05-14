using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Insurance;

public sealed class InsurancePlanFormViewModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string CoverageType { get; set; } = string.Empty;
}
