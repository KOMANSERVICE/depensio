namespace depensio.Application.UseCases.Auth.Services;

public class UserService(UserManager<ApplicationUser> _userManager) : IUserService
{
    public async Task<bool> EmailExists(string email)
    {
        if (string.IsNullOrEmpty(email)) return true;
        var user = await _userManager.FindByEmailAsync(email);
        return user is null;
    }
}
