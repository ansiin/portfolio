using DentalSaaS.Domain.Entities;

namespace DentalSaaS.Application.Abstractions;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct = default);
}
