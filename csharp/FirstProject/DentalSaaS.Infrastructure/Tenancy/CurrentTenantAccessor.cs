using DentalSaaS.Application.Abstractions;

namespace DentalSaaS.Infrastructure.Tenancy;

public sealed class CurrentTenantAccessor : ICurrentTenantAccessor
{
    private static readonly AsyncLocal<CurrentTenantContext?> Holder = new();

    public CurrentTenantContext Current => Holder.Value ?? new CurrentTenantContext { IsResolved = false };

    public void Set(CurrentTenantContext context)
    {
        Holder.Value = context;
    }
}
