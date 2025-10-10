using depensio.Application.UseCases.Auth.Services;

namespace depensio.Application.UseCases.Auth.Commands.CreateUser;

public record CreateUserCommand(SignUpBoutiqueDTO Signup)
    : ICommand<CreateUserResult>;

public record CreateUserResult(bool Result);

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserService _userService;
    public CreateUserCommandValidator(IUserService userService)
    {
        _userService = userService;

        RuleFor(x => x.Signup.Email)
            .NotEmpty().WithMessage("L'adresse email est obligatoire.")
            .EmailAddress().WithMessage("L'adresse email n'est pas valide.");
        RuleFor(x => x.Signup.BoutiqueId).NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }

}
