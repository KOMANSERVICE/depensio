namespace depensio.Application.UseCases.Auth.Commands.UpdateUser;

public class UpdateUserHandler(
        UserManager<ApplicationUser> _userManager,
        IEncryptionService _encryptionService,
        IUserContextService _userContextService
    ) : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{

    public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userInfos = request.UserInfos;

        var userId = _userContextService.GetUserId();
        var email = _userContextService.GetEmail();

        if(email != userInfos.Email)
            throw new UnauthorizedException("Vous n'êtes pas autorisé à mettre à jour cet utilisateur");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("Utilisateur non trouvé");
              

        // Met à jour les propriétés
        user.LastName = _encryptionService.Encrypt(userInfos.LastName);
        user.FirstName = _encryptionService.Encrypt(userInfos.FirstName);
        user.PhoneNumber = _encryptionService.Encrypt(userInfos.Tel);

        // Sauvegarde les modifications
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Erreur lors de la mise à jour : {errors}");
        }

        return new UpdateUserResult(true);
    }
}
