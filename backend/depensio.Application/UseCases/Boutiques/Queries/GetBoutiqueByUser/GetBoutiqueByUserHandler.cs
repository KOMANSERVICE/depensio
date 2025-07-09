namespace depensio.Application.UserCases.Boutiques.Queries.GetBoutiqueByUser;


public class GetBoutiqueByUserHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetBoutiqueByUserQuery, GetBoutiqueByUserResult>
{
    public async Task<GetBoutiqueByUserResult> Handle(GetBoutiqueByUserQuery request, CancellationToken cancellationToken)
    {

        var userId = _userContextService.GetUserId();
        var userBoutique = await dbContext.Boutiques
        .Where(b => b.UsersBoutiques.Any(ub => ub.UserId == userId))
        .Select(b => new BoutiqueDTO(
            b.Id.Value,
            b.Name,
            b.Location
        ))
        .ToListAsync();


        return new GetBoutiqueByUserResult(userBoutique);
    }
}
