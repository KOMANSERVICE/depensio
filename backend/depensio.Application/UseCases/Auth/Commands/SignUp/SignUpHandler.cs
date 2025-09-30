using depensio.Application.Interfaces;
using depensio.Application.UseCases.Auth.Services;

namespace depensio.Application.UserCases.Auth.Commands.SignUp;

public class SignUpHandler(
    UserManager<ApplicationUser> _userManager,
    IEncryptionService _encryptionService,
    IUserService _userService
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

            await _userService.GenerateEmailConfirmationTokenAsync(userM);
            
            return new SignUpResult(true);
        }
        return new SignUpResult(false);
    }


}
