namespace App.BLL.Abstractions;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? GetUserIdOrDefault();
    Guid GetRequiredUserId();
}
