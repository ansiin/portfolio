using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _db;

    public PatientRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<Patient>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default)
        => await _db.Patients
            .Where(p => p.CompanyId == companyId)
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .ToArrayAsync(ct);

    public Task<Patient?> GetAsync(Guid companyId, Guid id, CancellationToken ct = default)
        => _db.Patients.SingleOrDefaultAsync(p => p.CompanyId == companyId && p.Id == id, ct);

    public async Task AddAsync(Patient patient, CancellationToken ct = default)
    {
        await _db.Patients.AddAsync(patient, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Patient patient, CancellationToken ct = default)
    {
        _db.Patients.Update(patient);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Patient patient, string userId, CancellationToken ct = default)
    {
        patient.IsDeleted = true;
        patient.DeletedAt = DateTimeOffset.UtcNow;
        patient.DeletedBy = userId;
        _db.Patients.Update(patient);
        await _db.SaveChangesAsync(ct);
    }
}
