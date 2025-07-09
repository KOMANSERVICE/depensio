using depensio.Application.UserCases.Auth.DTOs;

namespace depensio.Application.UserCases.Auth.Queries.SignIn;

public record SignInQuery(SignInDTO Signin)
    :IQuery<SignInResult>;

public record SignInResult(string Token);


public class SignInQueryValidator : AbstractValidator<SignInQuery>
{
    public SignInQueryValidator()
    {
            RuleFor(x => x.Signin).NotNull().NotEmpty().WithMessage("Connaction fail");
            RuleFor(x => x.Signin.Email).NotNull().NotEmpty().WithMessage("Connaction fail");
            RuleFor(x => x.Signin.Password).NotNull().NotEmpty().WithMessage("Connaction fail");
    }
}