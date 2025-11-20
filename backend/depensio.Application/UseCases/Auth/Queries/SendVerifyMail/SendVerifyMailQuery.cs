namespace depensio.Application.UseCases.Auth.Queries.SendVerifyMail;

public record SendVerifyMailQuery(string UserId)
    : IQuery<SendVerifyMailResult>;

public record SendVerifyMailResult(bool Success);

public class SendVerifyMailValidator
    : AbstractValidator<SendVerifyMailQuery>
{
    public SendVerifyMailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID cannot be empty.");
    }
}