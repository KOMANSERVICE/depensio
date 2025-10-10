
using depensio.Application.Interfaces;
using depensio.Application.Services;
using depensio.Application.UserCases.Boutiques.Queries.GetBoutiqueByUser;
using Microsoft.EntityFrameworkCore;

namespace depensio.Application.UseCases.Boutiques.Queries.GetOneBoutiqueByUser;

public class GetOneBoutiqueByUserHandler(
        IDepensioDbContext dbContext,
        IUserContextService _userContextService
    )
    : IQueryHandler<GetOneBoutiqueByUserQuery, GetOneBoutiqueByUserResult>
{
    public async Task<GetOneBoutiqueByUserResult> Handle(GetOneBoutiqueByUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        var userBoutique = await dbContext.Boutiques
        .Where(b => b.UsersBoutiques.Any(ub => ub.UserId == userId) && b.Id == BoutiqueId.Of(request.BoutiqueId))
        .Select(b => new BoutiqueDTO
        {
            Id = b.Id.Value,
            Name = b.Name,
            Location = b.Location,
            CreatedAt = DateOnly.FromDateTime(b.CreatedAt)
        })
        .FirstOrDefaultAsync() ?? new BoutiqueDTO();


        return new GetOneBoutiqueByUserResult(userBoutique);
    }
}
