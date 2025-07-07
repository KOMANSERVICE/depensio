namespace depensio.Application.Auth.Commands.VerifyMail;

public class VerifyMailHandler(
        UserManager<ApplicationUser> _userManager
    )
    : ICommandHandler<VerifyMailCommand, VerifyMailResult>
{
    public async Task<VerifyMailResult> Handle(VerifyMailCommand command, CancellationToken cancellationToken)
    {
        var verifyMail = command.VerifyMail;

        var user = await _userManager.FindByIdAsync(verifyMail.Id);         
        var result = await _userManager.ConfirmEmailAsync(user!, verifyMail.Code);

        if (result.Succeeded){
            await _userManager.SetLockoutEnabledAsync(user!,false);
            return new VerifyMailResult(true); 
        }

        throw new BadRequestException("Vérification fail");
    }


}
