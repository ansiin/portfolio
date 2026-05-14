using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Billing;

public sealed class InvoiceFormViewModel
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }
}
