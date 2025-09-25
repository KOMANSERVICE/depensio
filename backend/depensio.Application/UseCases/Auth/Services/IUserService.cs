
namespace depensio.Application.UseCases.Auth.Services;

public interface IUserService
{
    Task<bool> EmailExists(string email);
}
