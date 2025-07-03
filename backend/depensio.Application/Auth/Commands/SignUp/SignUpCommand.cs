

namespace depensio.Application.Auth.Commands.SignUp;

public record SignUpCommand(SignUpDTO Signup) 
    : ICommand<SignUpResult>;

public record SignUpResult(bool Result);

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public SignUpCommandValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Signup.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not correct")
            .MustAsync(async (request, email, cancellationToken) => {return await EmailExists(email);}).WithMessage("Email already existe"); ;
        RuleFor(x => x.Signup.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(x => x.Signup.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(x => x.Signup.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password's lenght must be greater than 6");
        RuleFor(x => x.Signup.ConfirmPasswords).MinimumLength(6).WithMessage("ConfirmPassword's lenght must be greater than 6");
        RuleFor(x => x.Signup.Password).Equal(x => x.Signup.ConfirmPasswords).WithMessage("Password  and ConfirmPassword must be equal");
    }

    private async Task<bool> EmailExists(string email)
    {
        if(string.IsNullOrEmpty(email)) return true;
        var user = await _userManager.FindByEmailAsync(email);
        return user is null;
    }

}