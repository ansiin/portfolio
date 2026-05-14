using DentalSaaS.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace DentalSaaS.Infrastructure.Security;

public sealed class TenantResolvedAuthorizationHandler : AuthorizationHandler<TenantResolvedRequirement>
{
    private readonly ICurrentTenantAccessor _tenant;

    public TenantResolvedAuthorizationHandler(ICurrentTenantAccessor tenant)
    {
        _tenant = tenant;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantResolvedRequirement requirement)
    {
        if (_tenant.Current.IsResolved)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
