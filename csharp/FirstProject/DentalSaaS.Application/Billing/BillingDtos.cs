namespace DentalSaaS.Application.Billing;

public sealed record InvoiceDto(Guid Id, Guid PatientId, decimal Amount, bool IsPaid);
public sealed record CreateInvoiceRequest(Guid PatientId, decimal Amount);
