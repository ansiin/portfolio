namespace DentalSaaS.Application.Abstractions;

public sealed class CurrentTenantContext
{
    public bool IsResolved { get; init; }
    public Guid CompanyId { get; init; }
    public string TenantSlug { get; init; } = string.Empty;
}
