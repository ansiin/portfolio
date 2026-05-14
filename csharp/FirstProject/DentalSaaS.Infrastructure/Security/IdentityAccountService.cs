using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace DentalSaaS.Infrastructure.Security;

public sealed class IdentityAccountService : IIdentityAccountService
{
    private readonly UserManager<AppUser> _users;

    public IdentityAccountService(UserManager<AppUser> users)
    {
        _users = users;
    }

    public async Task<Result<string>> CreateUserAsync(
        string email,
        string displayName,
        string password,
        IEnumerable<string>? globalRoles = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return Result<string>.Failure("Email and password are required.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existing = await _users.FindByEmailAsync(normalizedEmail);
        if (existing is not null)
        {
            return Result<string>.Failure("User already exists.");
        }

        var user = new AppUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            DisplayName = displayName.Trim()
        };

        var create = await _users.CreateAsync(user, password);
        if (!create.Succeeded)
        {
            return Result<string>.Failure(string.Join("; ", create.Errors.Select(e => e.Description)));
        }

        if (globalRoles is not null)
        {
            var roles = globalRoles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (roles.Length > 0)
            {
                var addRoles = await _users.AddToRolesAsync(user, roles);
                if (!addRoles.Succeeded)
                {
                    return Result<string>.Failure(string.Join("; ", addRoles.Errors.Select(e => e.Description)));
                }
            }
        }

        return Result<string>.Success(user.Id.ToString());
    }
}
