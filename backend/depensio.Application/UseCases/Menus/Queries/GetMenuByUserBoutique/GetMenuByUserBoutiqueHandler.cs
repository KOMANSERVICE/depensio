using depensio.Application.Services;
using depensio.Application.UseCases.Menus.DTOs;

namespace depensio.Application.UseCases.Menus.Queries.GetMenuByUserBoutique;

public class GetMenuByUserBoutiqueHandler(
        IDepensioDbContext _dbContext,
        IUserContextService _userContextService,
        MenuService _menuService
    )
    : IQueryHandler<GetMenuByUserBoutiqueQuery, GetMenuByUserBoutiqueResult>
{
    public async Task<GetMenuByUserBoutiqueResult> Handle(GetMenuByUserBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        
        var menus = await _menuService.GetMenuByUserBoutiqueAsync(userId, request.BoutiqueId);

        return new GetMenuByUserBoutiqueResult(menus);
    }
}
