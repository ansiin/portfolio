namespace DentalSaaS.Application.PaymentPlans;

public sealed record PaymentPlanDto(Guid Id, Guid InvoiceId, int Months, decimal MonthlyAmount, DateOnly StartDate);
public sealed record CreatePaymentPlanRequest(Guid InvoiceId, int Months, decimal MonthlyAmount, DateOnly StartDate);
