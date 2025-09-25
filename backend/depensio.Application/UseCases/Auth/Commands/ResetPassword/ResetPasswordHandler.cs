
using Microsoft.AspNetCore.Identity;

namespace depensio.Application.UseCases.Auth.Commands.ResetPassword;

public class ResetPasswordHandler(
    UserManager<ApplicationUser> _userManager
    )
    : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var requestModel = request.ResetPassword;
        var user = await _userManager.FindByIdAsync(requestModel.Id);
        if (user is null)
            throw new NotFoundException("Utilisateur non trouver");

        var decodedToken = Uri.UnescapeDataString(requestModel.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, requestModel.NewPassword); 
        if (result.Succeeded)
            return new ResetPasswordResult(true);
        // Ajout pour afficher les erreurs
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));

        throw new BadRequestException("Erreur lors de la réinitialisation du mot de passe");
    }
}
