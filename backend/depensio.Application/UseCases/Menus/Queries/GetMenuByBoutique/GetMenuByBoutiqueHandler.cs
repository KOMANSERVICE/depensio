
using depensio.Application.UseCases.Menus.DTOs;

namespace depensio.Application.UseCases.Menus.Queries.GetMenuByBoutique;

public class GetMenuByBoutiqueHandler(
        MenuService menuService
    )
    : IQueryHandler<GetMenuByBoutiqueQuery, GetMenuByBoutiqueResult>
{
    public async Task<GetMenuByBoutiqueResult> Handle(GetMenuByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        //TODO : A reimplementer avec boutiqueId
        var menus = await menuService.GetMenuByBoutiqueAsync();

        return new GetMenuByBoutiqueResult(menus);
    }
}
