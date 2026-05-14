using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Abstractions;

public interface IFeatureGateService
{
    Task<Result> EnsureAllowedAsync(Guid companyId, Feature feature, CancellationToken ct = default);
}
