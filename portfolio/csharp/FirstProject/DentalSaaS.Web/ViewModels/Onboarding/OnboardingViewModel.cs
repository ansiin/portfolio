using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Onboarding;

public sealed class OnboardingViewModel
{
    [Required]
    [StringLength(120)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string TenantSlug { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string OwnerEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string OwnerDisplayName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string OwnerPassword { get; set; } = string.Empty;
}
