using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Treatments;

public interface ITreatmentService
{
    Task<IReadOnlyCollection<TreatmentDto>> ListAsync(CancellationToken ct = default);
    Task<TreatmentDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreateTreatmentRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateTreatmentRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
