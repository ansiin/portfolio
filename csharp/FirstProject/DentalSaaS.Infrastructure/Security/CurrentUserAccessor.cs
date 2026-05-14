using DentalSaaS.Application.Abstractions;

namespace DentalSaaS.Infrastructure.Security;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private static readonly AsyncLocal<CurrentUserContext?> Holder = new();

    public CurrentUserContext Current => Holder.Value ?? new CurrentUserContext();

    public void Set(CurrentUserContext context)
    {
        Holder.Value = context;
    }
}
