using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Management;

public sealed class CompanySettingsFormViewModel
{
    [Required]
    [StringLength(3, MinimumLength = 2)]
    public string CountryCode { get; set; } = "US";

    [Range(30, 730)]
    public int XrayIntervalDays { get; set; } = 180;
}
