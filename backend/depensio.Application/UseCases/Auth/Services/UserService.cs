using depensio.Application.Interfaces;
using IDR.Library.BuildingBlocks.Security.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace depensio.Application.UseCases.Auth.Services;

public class UserService(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    ISecureSecretProvider _secureSecretProvider
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
        var Frontend_BaseUrl = _configuration["Frontend:BaseUrl"]!;
        var BaseUrl = await _secureSecretProvider.GetSecretAsync(Frontend_BaseUrl);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
        var values = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "date", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") },
                { "link", $"{BaseUrl}/verifier-mail/{user.Id}?code={encodedToken}" }
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
