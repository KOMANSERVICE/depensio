using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace depensio.Application.UserCases.Auth.Commands.VerifyMail;

public class VerifyMailHandler(
        UserManager<ApplicationUser> _userManager
    )
    : ICommandHandler<VerifyMailCommand, VerifyMailResult>
{
    public async Task<VerifyMailResult> Handle(VerifyMailCommand command, CancellationToken cancellationToken)
    {
        var verifyMail = command.VerifyMail;
        var encoded = verifyMail.Code;
        byte[] tokenBytes;

        tokenBytes = WebEncoders.Base64UrlDecode(encoded) ?? throw new BadRequestException("Vérification fail.");
      
        var token = Encoding.UTF8.GetString(tokenBytes);
        var user = await _userManager.FindByIdAsync(verifyMail.Id);         
        var result = await _userManager.ConfirmEmailAsync(user!, token);

        if (result.Succeeded){
            await _userManager.SetLockoutEnabledAsync(user!,false);
            return new VerifyMailResult(true); 
        }

        throw new BadRequestException("Vérification fail");
    }


}
