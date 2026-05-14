using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.PaymentPlans;

public sealed class PaymentPlanFormViewModel
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    [Range(1, 120)]
    public int Months { get; set; }

    [Required]
    [Range(0.01, 100000)]
    public decimal MonthlyAmount { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }
}
