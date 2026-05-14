using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;

namespace DentalSaaS.Infrastructure.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;

    public AuditLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLog log, CancellationToken ct = default)
    {
        await _db.AuditLogs.AddAsync(log, ct);
        await _db.SaveChangesAsync(ct);
    }
}
