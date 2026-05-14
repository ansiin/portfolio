using System.Security.Claims;
using App.BLL.Abstractions;

namespace WebApp.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => GetPrincipal()?.Identity?.IsAuthenticated ?? false;

    public Guid? GetUserIdOrDefault()
    {
        var userIdString = GetPrincipal()?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdString, out var userId) ? userId : null;
    }

    public Guid GetRequiredUserId()
    {
        return GetUserIdOrDefault() ?? throw new InvalidOperationException("Authenticated user id is required.");
    }

    private ClaimsPrincipal? GetPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}
