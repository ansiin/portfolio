using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.PracticeSetup;

public sealed class TreatmentTypeFormViewModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(5, 600)]
    public int DurationMinutes { get; set; }

    [Required]
    [Range(0.01, 100000)]
    public decimal Price { get; set; }
}
