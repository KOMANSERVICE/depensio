using depensio.Application.UseCases.Auth.Services;
using depensio.Application.UserCases.Auth.Commands.SignUp;

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
        RuleFor(x => x.Signup.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(x => x.Signup.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(x => x.Signup.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");
    }

}
