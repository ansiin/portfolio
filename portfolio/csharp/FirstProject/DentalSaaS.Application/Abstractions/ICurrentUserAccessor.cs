namespace DentalSaaS.Application.Abstractions;

public interface ICurrentUserAccessor
{
    CurrentUserContext Current { get; }
    void Set(CurrentUserContext context);
}
