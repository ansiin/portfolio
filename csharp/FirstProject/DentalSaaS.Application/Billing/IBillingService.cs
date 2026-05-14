using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Billing;

public interface IBillingService
{
    Task<IReadOnlyCollection<InvoiceDto>> ListAsync(CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreateInvoiceRequest request, CancellationToken ct = default);
    Task<Result> MarkPaidAsync(Guid invoiceId, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid invoiceId, CancellationToken ct = default);
}
