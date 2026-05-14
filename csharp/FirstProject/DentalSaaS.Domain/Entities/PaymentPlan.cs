using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class PaymentPlan : CompanyEntityBase
{
    public Guid InvoiceId { get; set; }
    public int Months { get; set; }
    public decimal MonthlyAmount { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
