namespace depensio.Application.Auth.Queries.SignIn;

public record SignInQuery(SignInDTO SignIn)
    :IQuery<SignInResult>;

public record SignInResult(string Token);


public class SignInQueryValidator : AbstractValidator<SignInQuery>
{
    public SignInQueryValidator()
    {
            RuleFor(x => x.SignIn).NotNull().NotEmpty().WithMessage("Connaction fail");
            RuleFor(x => x.SignIn.Email).NotNull().NotEmpty().WithMessage("Connaction fail");
            RuleFor(x => x.SignIn.Password).NotNull().NotEmpty().WithMessage("Connaction fail");
    }
}