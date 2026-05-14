using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Abstractions;

public interface IIdentityAccountService
{
    Task<Result<string>> CreateUserAsync(
        string email,
        string displayName,
        string password,
        IEnumerable<string>? globalRoles = null,
        CancellationToken ct = default);
}
