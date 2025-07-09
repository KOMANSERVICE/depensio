using depensio.Application.Interfaces;
using depensio.Application.UserCases.Boutiques.DTOs;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UserCases.Boutiques.Commands.CreateBoutique;

public class CreateBoutiqueHandler(
    IGenericRepository<Boutique> _boutiqueRepository,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService
    )
    : ICommandHandler<CreateBoutiqueCommand, CreateBoutiqueResult>
{
    public async Task<CreateBoutiqueResult> Handle(
        CreateBoutiqueCommand command,
        CancellationToken cancellationToken
        )
    {
        var boutique = CreateNewBoutique(command.Boutique);

        await _boutiqueRepository.AddDataAsync(boutique, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new CreateBoutiqueResult(boutique.Id.Value);
    }

    private Boutique CreateNewBoutique(BoutiqueDTO boutiqueDTO)
    {
        var boutiqueId = BoutiqueId.Of(Guid.NewGuid());
        var userId = _userContextService.GetUserId();

        return new Boutique
        {
            Id = boutiqueId,
            Name = boutiqueDTO.Name,
            Location = boutiqueDTO.Location,
            OwnerId = userId,
            UsersBoutiques = new List<UsersBoutique>
        {
            new UsersBoutique
            {
                Id = UsersBoutiqueId.Of(Guid.NewGuid()),
                UserId = userId,
                BoutiqueId = boutiqueId
            }
        }
        };
    }
}