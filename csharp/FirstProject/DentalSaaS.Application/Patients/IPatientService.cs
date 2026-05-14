using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Patients;

public interface IPatientService
{
    Task<IReadOnlyCollection<PatientListItem>> ListAsync(CancellationToken ct = default);
    Task<PatientListItem?> GetAsync(Guid patientId, CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreatePatientRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdatePatientRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid patientId, CancellationToken ct = default);
}
