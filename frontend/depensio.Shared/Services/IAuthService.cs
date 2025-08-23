
using depensio.Shared.Pages.Auth.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IAuthService
{
    Task<bool> SignInAsync(SignInRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task LoadTokenAsync();
}
