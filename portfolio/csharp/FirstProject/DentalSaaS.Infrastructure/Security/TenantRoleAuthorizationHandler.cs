using DentalSaaS.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace DentalSaaS.Infrastructure.Security;

public sealed class TenantRoleAuthorizationHandler : AuthorizationHandler<TenantRoleRequirement>
{
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;

    public TenantRoleAuthorizationHandler(ICurrentTenantAccessor tenant, ICurrentUserAccessor user)
    {
        _tenant = tenant;
        _user = user;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRoleRequirement requirement)
    {
        if (!_tenant.Current.IsResolved)
        {
            return Task.CompletedTask;
        }

        var activeRole = _user.Current.ActiveTenantRole;
        if (!string.IsNullOrWhiteSpace(activeRole) &&
            requirement.Roles.Contains(activeRole, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
