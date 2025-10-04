
using depensio.Application.UseCases.Profiles.DTO;

namespace depensio.Application.UseCases.Profiles.Commands.AssignedProfileToUser;

public class AssignedProfileToUserHandler(
        IGenericRepository<UsersBoutique> _usersBoutiqueRepository,
        IUserContextService _userContextService,
        IUnitOfWork _unitOfWork
    ) : ICommandHandler<AssignedProfileToUserCommand, AssignedProfileToUserResult>
{
    public async Task<AssignedProfileToUserResult> Handle(AssignedProfileToUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        var id = await UpdateUserBoutique(request.BoutiqueId, userId, request.AssigneProfile);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new AssignedProfileToUserResult(id);
    }

    private async Task<Guid> UpdateUserBoutique(Guid boutiqueId, string userId, AssigneProfileDTO assigneProfile)
    {
        var userBoutique = await _usersBoutiqueRepository
            .FindAsync(ub => ub.BoutiqueId == BoutiqueId.Of(boutiqueId)
            && ub.UserId == userId) 
            ?? throw new BadRequestException("User not found in this boutique");

        userBoutique.ProfileId = ProfileId.Of(assigneProfile.ProfileId);

        _usersBoutiqueRepository.UpdateData(userBoutique);

        return userBoutique.Id.Value;
    }

}

