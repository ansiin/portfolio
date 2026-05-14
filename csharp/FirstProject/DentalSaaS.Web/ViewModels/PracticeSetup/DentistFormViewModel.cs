using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.PracticeSetup;

public sealed class DentistFormViewModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LicenseNumber { get; set; } = string.Empty;
}
