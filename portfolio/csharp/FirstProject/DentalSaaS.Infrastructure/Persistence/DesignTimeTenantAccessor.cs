using DentalSaaS.Application.Abstractions;

namespace DentalSaaS.Infrastructure.Persistence;

public sealed class DesignTimeTenantAccessor : ICurrentTenantAccessor
{
    private CurrentTenantContext _context = new() { IsResolved = false };

    public CurrentTenantContext Current => _context;

    public void Set(CurrentTenantContext context)
    {
        _context = context;
    }
}
