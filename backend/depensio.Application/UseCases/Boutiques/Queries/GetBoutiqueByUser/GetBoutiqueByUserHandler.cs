using depensio.Application.Services;

namespace depensio.Application.UserCases.Boutiques.Queries.GetBoutiqueByUser;


public class GetBoutiqueByUserHandler(
        IDepensioDbContext dbContext,
        IUserContextService _userContextService,
        MenuService _menuService
    ) : IQueryHandler<GetBoutiqueByUserQuery, GetBoutiqueByUserResult>
{
    public async Task<GetBoutiqueByUserResult> Handle(GetBoutiqueByUserQuery request, CancellationToken cancellationToken)
    {

        var userId = _userContextService.GetUserId();
        var userBoutique = await dbContext.Boutiques
        .Where(b => b.UsersBoutiques.Any(ub => ub.UserId == userId))      
        .Select(b => new BoutiqueDTO{
            Id= b.Id.Value,
            Name = b.Name,
            Location = b.Location,
            CreatedAt = DateOnly.FromDateTime(b.CreatedAt)
        })
        .ToListAsync();

        foreach (var b in userBoutique)
        {
            var menus = await _menuService.GetMenuByUserBoutiqueAsync(userId, b.Id);
            if(menus != null && menus.Any())
                b.FirstUrl = menus.FirstOrDefault()?.UrlFront ?? "";
        } 
        

        return new GetBoutiqueByUserResult(userBoutique);
    }
}
