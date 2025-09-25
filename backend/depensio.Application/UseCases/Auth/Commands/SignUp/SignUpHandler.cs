using depensio.Application.Interfaces;

namespace depensio.Application.UserCases.Auth.Commands.SignUp;

public class SignUpHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    IEncryptionService _encryptionService,
    ITemplateRendererService _templateRendererService
    )
    : ICommandHandler<SignUpCommand, SignUpResult>
{
    public async Task<SignUpResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        
        var signUp = request.Signup;
        var user = new ApplicationUser
        {
            UserName = signUp.Email,
            Email = signUp.Email,
            LastName = _encryptionService.Encrypt(signUp.LastName),
            FirstName = _encryptionService.Encrypt(signUp.FirstName),
            PhoneNumber = _encryptionService.Encrypt(signUp.Tel)
        };


        var result = await _userManager.CreateAsync(user, signUp.Password);
        if (result.Succeeded)
        {

            var userM = await _userManager.FindByEmailAsync(signUp.Email);
            if(userM is  null)
                return new SignUpResult(false);
            //var role = await roleManager.RoleExistsAsync("User");
            //if (!role)
            //{
            //    await _roleManager.CreateAsync(new IdentityRole()
            //    {
            //        Id = "User",
            //        Name = "User"
            //    });
            //}
            //await _userManager.AddToRoleAsync(userM, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userM);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var values = new Dictionary<string, string>
            {
                { "email", userM.Email },
                { "date", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") },
                { "link", $"{_configuration["JWT:ValidIssuer"]}/verifier-mail/{user.Id}?code={encodedToken}" }
            };

            var mailContent = await _templateRendererService.RenderTemplateAsync("AccountCreated.html", values);

            var mail = new EmailModel
            {
                ToMailIds = new List<string>()
                    {
                        userM.Email
                    },
                Suject = mailContent.Subject,
                Body = mailContent.Body
            };
            
            await _mailService.SendEmailAsync(mail);
            return new SignUpResult(true);
        }
        return new SignUpResult(false);
    }


}
