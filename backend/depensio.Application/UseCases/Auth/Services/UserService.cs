using depensio.Application.Interfaces;

namespace depensio.Application.UseCases.Auth.Services;

public class UserService(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration
    ) : IUserService
{
    public async Task<bool> EmailExists(string email)
    {
        if (string.IsNullOrEmpty(email)) return true;
        var user = await _userManager.FindByEmailAsync(email);
        return user is null;
    }

    public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var values = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "date", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") },
                { "link", $"{_configuration["JWT:ValidIssuer"]}/verifier-mail/{user.Id}?code={encodedToken}" }
            };

        var mailContent = await _mailService.RenderHtmlTemplateAsync("AccountCreated.html", values);

        var mail = new EmailModel
        {
            ToMailIds = new List<string>()
                    {
                        user.Email
                    },
            Suject = mailContent.Subject,
            Body = mailContent.Body
        };

        await _mailService.SendEmailAsync(mail);
    }
}
