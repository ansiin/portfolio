using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class Invoice : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
}
