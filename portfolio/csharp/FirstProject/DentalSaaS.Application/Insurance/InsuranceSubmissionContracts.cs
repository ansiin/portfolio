using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Application.Insurance;

public sealed record InsuranceSubmissionRequest(
    Guid EstimateId,
    Guid CompanyId,
    Guid PatientId,
    string CountryTemplate,
    decimal TotalAmount,
    string ProviderName,
    string PayloadJson);

public sealed record InsuranceSubmissionResult(
    bool IsSuccess,
    InsuranceSubmissionState SubmissionState,
    string? ExternalSubmissionId,
    string? Message);

public interface IInsuranceSubmissionGateway
{
    Task<InsuranceSubmissionResult> SubmitAsync(InsuranceSubmissionRequest request, CancellationToken ct = default);
}
