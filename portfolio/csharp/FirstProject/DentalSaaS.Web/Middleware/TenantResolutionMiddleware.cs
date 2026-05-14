using System.Security.Claims;
using DentalSaaS.Application.Abstractions;

namespace DentalSaaS.Web.Middleware;

public sealed class TenantResolutionMiddleware
{
    private static readonly HashSet<string> ExcludedRoots =
    [
        "",
        "account",
        "onboarding",
        "home",
        "system",
        "css",
        "js",
        "lib"
    ];

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICompanyRepository companies,
        ICompanyMembershipRepository memberships,
        ICurrentTenantAccessor currentTenant,
        ICurrentUserAccessor currentUser)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.Identity?.Name
            ?? "anonymous";

        var email = context.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var claimsRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

        currentUser.Set(new CurrentUserContext
        {
            UserId = userId,
            Email = email,
            Roles = claimsRoles,
            ActiveTenantRole = null,
            IsImpersonating = string.Equals(context.User.FindFirst("impersonating")?.Value, "true", StringComparison.OrdinalIgnoreCase),
            ImpersonatorUserId = context.User.FindFirst("impersonator_user_id")?.Value
        });

        var path = context.Request.Path.Value?.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries)
                   ?? [];

        if (path.Length == 0 || ExcludedRoots.Contains(path[0].ToLowerInvariant()))
        {
            currentTenant.Set(new CurrentTenantContext { IsResolved = false });
            await _next(context);
            return;
        }

        var slug = path[0].ToLowerInvariant();
        var company = await companies.GetBySlugAsync(slug, context.RequestAborted);

        if (company is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Tenant not found.");
            return;
        }

        if (!company.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Tenant is deactivated.");
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString.Value);
            context.Response.Redirect($"/account/login?returnUrl={returnUrl}");
            return;
        }

        var isMember = await memberships.IsMemberAsync(company.Id, userId, context.RequestAborted);
        if (!isMember)
        {
            context.Response.Redirect("/account/access-denied");
            return;
        }

        var tenantRole = await memberships.GetRoleAsync(company.Id, userId, context.RequestAborted);
        if (string.IsNullOrWhiteSpace(tenantRole))
        {
            context.Response.Redirect("/account/access-denied");
            return;
        }

        currentUser.Set(new CurrentUserContext
        {
            UserId = userId,
            Email = email,
            Roles = claimsRoles.Union([tenantRole]).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            ActiveTenantRole = tenantRole,
            IsImpersonating = string.Equals(context.User.FindFirst("impersonating")?.Value, "true", StringComparison.OrdinalIgnoreCase),
            ImpersonatorUserId = context.User.FindFirst("impersonator_user_id")?.Value
        });

        currentTenant.Set(new CurrentTenantContext
        {
            IsResolved = true,
            CompanyId = company.Id,
            TenantSlug = company.Slug
        });

        await _next(context);
    }
}
