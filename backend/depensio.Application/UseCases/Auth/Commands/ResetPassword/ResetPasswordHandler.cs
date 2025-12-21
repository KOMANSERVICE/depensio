
using depensio.Application.Interfaces;
using IDR.Library.BuildingBlocks.Security.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace depensio.Application.UseCases.Auth.Commands.ResetPassword;

public class ResetPasswordHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    ISecureSecretProvider _secureSecretProvider
    )
    : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var requestModel = request.ResetPassword;
        var user = await _userManager.FindByIdAsync(requestModel.Id);
        if (user is null)
            throw new NotFoundException("Utilisateur non trouver");

        var decodedToken = System.Web.HttpUtility.HtmlDecode(requestModel.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, requestModel.NewPassword);
        if (result.Succeeded)
        {
            await _userManager.SetLockoutEnabledAsync(user!, false);
            await SendMailAsync(user);
            return new ResetPasswordResult(true);
        }
        // Ajout pour afficher les erreurs
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));

        throw new BadRequestException("Erreur lors de la réinitialisation du mot de passe");
    }

    private async Task SendMailAsync(ApplicationUser user)
    {

        var Frontend_BaseUrl = _configuration["Frontend:BaseUrl"]!;
        var BaseUrl = await _secureSecretProvider.GetSecretAsync(Frontend_BaseUrl);
        var values = new Dictionary<string, string>
            {
                { "loginLink", $"{BaseUrl}/login" }
            };

        var mailContent = await _mailService.RenderHtmlTemplateAsync("ResetPasswordSucces.html", values);

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
