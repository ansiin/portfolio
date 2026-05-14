namespace DentalSaaS.Application.Insurance;

public sealed record InsurancePlanDto(Guid Id, string Name, string CoverageType);

public sealed record CostEstimateDto(
    Guid Id,
    Guid PatientId,
    Guid? InsurancePlanId,
    string CountryTemplate,
    decimal TotalAmount,
    string ClaimStatus,
    DentalSaaS.Shared.Enums.InsuranceSubmissionState SubmissionState,
    DateTimeOffset? SubmittedAt,
    string? ExternalSubmissionId,
    string ProviderName,
    string? ProviderResponseMessage);

public sealed record CreateInsurancePlanRequest(string Name, string CoverageType);

public sealed record CreateCostEstimateRequest(Guid PatientId, Guid? InsurancePlanId, string CountryTemplate, decimal TotalAmount);
