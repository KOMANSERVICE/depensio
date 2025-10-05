
using depensio.Application.Services;

namespace depensio.Application.UseCases.Menus.Queries.GetOneMenuByUserBoutique;

public class GetOneMenuByUserBoutiqueHandler(
        IUserContextService _userContextService,
        MenuService _menuService)
    : IQueryHandler<GetOneMenuByUserBoutiqueQuery, GetOneMenuByUserBoutiqueResult>
{
    public async Task<GetOneMenuByUserBoutiqueResult> Handle(GetOneMenuByUserBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();

        var menu = await _menuService.GetOneMenuByUserBoutiqueAsync(userId, request.BoutiqueId, request.CurrentPath);

        return new GetOneMenuByUserBoutiqueResult(menu);
    }
}
