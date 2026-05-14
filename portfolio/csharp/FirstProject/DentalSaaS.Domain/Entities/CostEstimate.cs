using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class CostEstimate : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public Guid? InsurancePlanId { get; set; }
    public string CountryTemplate { get; set; } = "US-DEFAULT";
    public decimal TotalAmount { get; set; }
    public string ClaimStatus { get; set; } = "Draft";
    public DentalSaaS.Shared.Enums.InsuranceSubmissionState SubmissionState { get; set; } = DentalSaaS.Shared.Enums.InsuranceSubmissionState.Draft;
    public DateTimeOffset? SubmittedAt { get; set; }
    public string? ExternalSubmissionId { get; set; }
    public string? SubmissionPayloadJson { get; set; }
    public string? ProviderResponseMessage { get; set; }
    public string ProviderName { get; set; } = "MockInsuranceGateway";
}
