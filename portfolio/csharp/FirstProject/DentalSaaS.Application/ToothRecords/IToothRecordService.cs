using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.ToothRecords;

public interface IToothRecordService
{
    Task<IReadOnlyCollection<ToothRecordDto>> ListAsync(CancellationToken ct = default);
    Task<ToothRecordDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreateToothRecordRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateToothRecordRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
