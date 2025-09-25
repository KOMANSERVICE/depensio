namespace depensio.Application.UseCases.Auth.Commands.ForgetPassword;

public class ForgotPasswordHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    ITemplateRendererService _templateRendererService
    )
    : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {

        var requestModel = request.ForgotPassword;
        var user = await _userManager.FindByEmailAsync(requestModel.Email);
        if (user is null)
            throw new NotFoundException("Utilisateur non trouvé");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);

        var values = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "date", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") },
                { "link", $"{_configuration["JWT:ValidIssuer"]}/reset-password/{user.Id}?code={encodedToken}" }
            };

        var mailContent = await _templateRendererService.RenderTemplateAsync("AccountCreated.html", values);

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

        return new ForgotPasswordResult(true);
    }
}
