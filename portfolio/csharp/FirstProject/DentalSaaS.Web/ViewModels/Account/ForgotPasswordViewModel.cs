using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Account;

public sealed class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
