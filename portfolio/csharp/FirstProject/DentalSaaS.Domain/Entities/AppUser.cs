using Microsoft.AspNetCore.Identity;

namespace DentalSaaS.Domain.Entities;

public sealed class AppUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}
