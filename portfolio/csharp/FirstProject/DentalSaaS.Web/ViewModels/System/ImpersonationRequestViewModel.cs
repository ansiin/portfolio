using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.System;

public sealed class ImpersonationRequestViewModel
{
    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public string TargetUserId { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Reason { get; set; } = string.Empty;
}
