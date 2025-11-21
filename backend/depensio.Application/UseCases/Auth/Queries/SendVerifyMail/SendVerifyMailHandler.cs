
using depensio.Application.UserCases.Auth.Commands.SignUp;

namespace depensio.Application.UseCases.Auth.Queries.SendVerifyMail;

public class SendVerifyMailHandler(
    UserManager<ApplicationUser> _userManager,
    IUserService _userService)
    : IQueryHandler<SendVerifyMailQuery, SendVerifyMailResult>
{
    public async Task<SendVerifyMailResult> Handle(SendVerifyMailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if(user == null)
        {
            user = await _userManager.FindByEmailAsync(request.UserId) ??
            throw new BadRequestException("Vérification fail.");
        }

        await _userService.GenerateEmailConfirmationTokenAsync(user);

        return new SendVerifyMailResult(true);
    }
}
