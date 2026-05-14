using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Xrays;

public sealed class XrayFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public DateTimeOffset TakenAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    [StringLength(500)]
    public string FileUrl { get; set; } = string.Empty;
}
