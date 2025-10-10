using depensio.Application.UserCases.Auth.DTOs;

namespace depensio.Application.UserCases.Auth.Commands.SignUp;

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
            .NotEmpty().WithMessage("L'adresse email est obligatoire.")
            .EmailAddress().WithMessage("L'adresse email n'est pas valide.")
            .MustAsync(async (request, email, cancellationToken) => {return await EmailExists(email);}).WithMessage("L'adresse email existe déjà."); ;
        RuleFor(x => x.Signup.FirstName).NotEmpty().WithMessage("Le prénom est obligatoire.");
        RuleFor(x => x.Signup.LastName).NotEmpty().WithMessage("Le nom est obligatoire.");
        RuleFor(x => x.Signup.Password).NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(6).WithMessage("La longueur du mot de passe doit être supérieure à 6 caractères.");
        RuleFor(x => x.Signup.ConfirmPasswords).MinimumLength(6).WithMessage("La longueur de la confirmation du mot de passe doit être supérieure à 6 caractères.");
        RuleFor(x => x.Signup.Password).Equal(x => x.Signup.ConfirmPasswords).WithMessage("Le mot de passe et la confirmation doivent être identiques");
    }

    private async Task<bool> EmailExists(string email)
    {
        if(string.IsNullOrEmpty(email)) return true;
        var user = await _userManager.FindByEmailAsync(email);
        return user is null;
    }

}