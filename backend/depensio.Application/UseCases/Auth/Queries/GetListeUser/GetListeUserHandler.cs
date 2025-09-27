using depensio.Application.UseCases.Sales.DTOs;
using Microsoft.EntityFrameworkCore;

namespace depensio.Application.UseCases.Auth.Queries.ListeUser;

public class GetListeUserHandler(
        IDepensioDbContext _dbContext,
        IUserContextService _userContextService,
        IEncryptionService _encryptionService
    )    
    : IQueryHandler<GetListeUserQuery, GetListeUserResult>
{
    public async Task<GetListeUserResult> Handle(GetListeUserQuery request, CancellationToken cancellationToken)
    {
        var boutiqueId = request.BoutiqueId;
        var userId = _userContextService.GetUserId();

        if(!await _dbContext.Boutiques
            .AnyAsync(b => b.Id == BoutiqueId.Of(boutiqueId)
                          && b.UsersBoutiques.Any(ub => ub.UserId == userId), cancellationToken))
        {
            throw new UnauthorizedException($"You do not have access to this boutique. {userId}");
        }

        var users = await _dbContext.UsersBoutiques
            .Where(ub => ub.BoutiqueId == BoutiqueId.Of(boutiqueId))
            .Select(ub => new SignUpBoutiqueDTO
            {
                Email = ub.User.Email ?? "",
                FirstName = _encryptionService.Decrypt(ub.User.FirstName),
                LastName = _encryptionService.Decrypt(ub.User.LastName),
                BoutiqueId = ub.BoutiqueId.Value
            })
            .ToListAsync(cancellationToken);

        return new GetListeUserResult(users);
    }
}
