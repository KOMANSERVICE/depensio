using depensio.Application.Interfaces;

namespace depensio.Application.Auth.Commands.SignUp;

public class SignUpHandler(
    UserManager<ApplicationUser> _userManager,
    IEmailService _mailService,
    IConfiguration _configuration,
    IEncryptionService _encryptionService
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

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userM);

            var mail = new EmailModel
            {
                ToMailIds = new List<string>()
                    {
                        userM.Email
                    },
                Suject = "Vérification de mail",
                Body = GetBodyMail(userM, code)
            };
            
            await _mailService.SendEmailAsync(mail);
            return new SignUpResult(true);
        }
        return new SignUpResult(false);
    }

    private string GetBodyMail(ApplicationUser user, string code)
    {
        return $"""
                <p>Bonjour,</p>
                <p>Bienvenue sur la plateforme de suivi des dépendes.</P>
                <p>Pour vérifier votre mail {user.Email} ,</p>
                <p><a href='{_configuration["JWT:ValidIssuer"]}/verifier-mail/{user.Id}?code={code}
                '>Cliquez ici</a> ou sur le lien ci-dessous</p>
                <p>{_configuration["JWT:ValidIssuer"]}/verifier-mail/{user.Id}?code={code}</p>
                """;
    }
}
