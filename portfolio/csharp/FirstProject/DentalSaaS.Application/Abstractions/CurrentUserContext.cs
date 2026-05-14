namespace DentalSaaS.Application.Abstractions;

public sealed class CurrentUserContext
{
    public string UserId { get; init; } = "anonymous";
    public string Email { get; init; } = string.Empty;
    public string[] Roles { get; init; } = [];
    public string? ActiveTenantRole { get; init; }
    public bool IsImpersonating { get; init; }
    public string? ImpersonatorUserId { get; init; }

    public bool IsInRole(string role)
        => (!string.IsNullOrWhiteSpace(ActiveTenantRole) && string.Equals(ActiveTenantRole, role, StringComparison.OrdinalIgnoreCase))
           || Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
}
