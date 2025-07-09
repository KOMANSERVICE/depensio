using depensio.Application.UserCases.Auth.DTOs;

namespace depensio.Application.UserCases.Auth.Commands.VerifyMail;

public record VerifyMailCommand(VerifyMailDTO VerifyMail)
    :ICommand<VerifyMailResult>;

public record VerifyMailResult(bool Result);


public class VerifyMailCommandValidator : AbstractValidator<VerifyMailCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public VerifyMailCommandValidator(
        UserManager<ApplicationUser> userManager
        )
    {
        _userManager = userManager;

        RuleFor(x => x.VerifyMail.Id).NotEmpty()
            .MustAsync(async (request, id, cancellationToken) => { return await IdExists(id); }).WithMessage("Verification fail");
    }

    private async Task<bool> IdExists(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user != null && !user.EmailConfirmed;
    }
}