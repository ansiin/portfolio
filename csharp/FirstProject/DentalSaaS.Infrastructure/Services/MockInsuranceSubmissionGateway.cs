using DentalSaaS.Application.Insurance;
using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Infrastructure.Services;

public sealed class MockInsuranceSubmissionGateway : IInsuranceSubmissionGateway
{
    public Task<InsuranceSubmissionResult> SubmitAsync(InsuranceSubmissionRequest request, CancellationToken ct = default)
    {
        var externalId = $"MOCK-{request.EstimateId:N}".ToUpperInvariant();
        return Task.FromResult(new InsuranceSubmissionResult(
            true,
            InsuranceSubmissionState.Submitted,
            externalId,
            "Submitted to mock insurance provider."));
    }
}
