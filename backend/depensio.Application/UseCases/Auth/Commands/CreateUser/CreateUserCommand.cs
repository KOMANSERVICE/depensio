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
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not correct");
        RuleFor(x => x.Signup.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");
    }

}
