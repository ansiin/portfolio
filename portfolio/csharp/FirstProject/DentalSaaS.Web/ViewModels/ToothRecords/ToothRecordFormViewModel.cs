using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.ToothRecords;

public sealed class ToothRecordFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public Guid PatientId { get; set; }

    [Range(1, 32)]
    public int ToothNumber { get; set; }

    [Required]
    [StringLength(120)]
    public string ConditionStatus { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}
