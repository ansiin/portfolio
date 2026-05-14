using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class CompanyMembership : EntityBase
{
    public Guid CompanyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
