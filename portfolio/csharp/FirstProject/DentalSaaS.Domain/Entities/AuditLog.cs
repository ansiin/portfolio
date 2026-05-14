using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class AuditLog : CompanyEntityBase
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    public string ChangedByUserId { get; set; } = string.Empty;
}
