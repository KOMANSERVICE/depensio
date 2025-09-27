namespace depensio.Application.Interfaces;

public interface IUserContextService
{
    string GetUserId();
    string? GetEmail();
}
