using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Xrays;

public interface IXrayService
{
    Task<IReadOnlyCollection<XrayDto>> ListAsync(CancellationToken ct = default);
    Task<XrayDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreateXrayRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateXrayRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<XrayOverdueSummaryDto> GetOverdueSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, int>> GetPatientOverdueDaysAsync(CancellationToken ct = default);
}
