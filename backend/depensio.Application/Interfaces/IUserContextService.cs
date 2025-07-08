namespace depensio.Application.Interfaces;

public interface IUserContextService
{
    Guid GetUserId();
    string? GetEmail();
}
