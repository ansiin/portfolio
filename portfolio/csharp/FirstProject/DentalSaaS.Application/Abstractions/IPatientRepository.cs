using DentalSaaS.Domain.Entities;

namespace DentalSaaS.Application.Abstractions;

public interface IPatientRepository
{
    Task<IReadOnlyCollection<Patient>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default);
    Task<Patient?> GetAsync(Guid companyId, Guid id, CancellationToken ct = default);
    Task AddAsync(Patient patient, CancellationToken ct = default);
    Task UpdateAsync(Patient patient, CancellationToken ct = default);
    Task SoftDeleteAsync(Patient patient, string userId, CancellationToken ct = default);
}
