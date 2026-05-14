namespace DentalSaaS.Application.Abstractions;

public interface ICurrentTenantAccessor
{
    CurrentTenantContext Current { get; }
    void Set(CurrentTenantContext context);
}
