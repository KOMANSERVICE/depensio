
using depensio.Application.UseCases.Profiles.DTO;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Profiles.Commands.CreateProfile;

public class CreateProfileHandler(
        IUnitOfWork _unitOfWork,
        IGenericRepository<Profile> _profileRepository,
        IDepensioDbContext _dbContext,
        IUserContextService _userContextService
    )
    : ICommandHandler<CreateProfileCommand, CreateProfileResult>
{
    public async Task<CreateProfileResult> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var boutiqueId = request.BoutiqueId;
        var userId = _userContextService.GetUserId();

        var profileExiste = _dbContext.Boutiques
                  .Any(b => b.Id == BoutiqueId.Of(boutiqueId)
                              && b.UsersBoutiques.Any(ub => ub.UserId == userId)
                              && b.Profiles.Any(p => p.Name == request.Profile.Name));
        if (profileExiste)
        {
            throw new BadRequestException($"Le profile {request.Profile.Name} existe déjà");
        }


        var id = this.AddProfile(boutiqueId, request.Profile);
        await _unitOfWork.SaveChangesDataAsync();

        return new CreateProfileResult(id);
    }

    private Guid AddProfile(Guid boutiqueId,ProfileDTO profileDTO)
    {
        var profileId = ProfileId.Of(Guid.NewGuid());

        var profileMenus = profileDTO.MenuIds.Select(m => new ProfileMenu()
        {
            ReferenceMenu = m.Reference,
            IsActive = m.IsActive,
            ProfileId = profileId
        }).ToList();

        _profileRepository.AddDataAsync(new Profile()
        {
            Id = profileId,
            Name = profileDTO.Name,
            BoutiqueId = BoutiqueId.Of(boutiqueId),
            ProfileMenus = profileMenus
        });

        return profileId.Value;
    }
}
