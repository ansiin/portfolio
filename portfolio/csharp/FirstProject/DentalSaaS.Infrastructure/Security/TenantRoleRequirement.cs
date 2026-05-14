using Microsoft.AspNetCore.Authorization;

namespace DentalSaaS.Infrastructure.Security;

public sealed class TenantRoleRequirement : IAuthorizationRequirement
{
    public TenantRoleRequirement(params string[] roles)
    {
        Roles = roles;
    }

    public IReadOnlyCollection<string> Roles { get; }
}
