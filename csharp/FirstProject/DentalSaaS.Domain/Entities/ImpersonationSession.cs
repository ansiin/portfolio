using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class ImpersonationSession : EntityBase
{
    public string AdminUserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndedAt { get; set; }
}
