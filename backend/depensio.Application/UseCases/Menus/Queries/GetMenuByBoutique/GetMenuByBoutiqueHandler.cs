
using depensio.Application.UseCases.Menus.DTOs;

namespace depensio.Application.UseCases.Menus.Queries.GetMenuByBoutique;

public class GetMenuByBoutiqueHandler(
        IDepensioDbContext _dbContext
    )
    : IQueryHandler<GetMenuByBoutiqueQuery, GetMenuByBoutiqueResult>
{
    public async Task<GetMenuByBoutiqueResult> Handle(GetMenuByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        //TODO : A reimplementer avec boutiqueId
        var menus = await _dbContext.Menus
            .Select(m => new MenuDTO(
                m.Id.Value,
                m.Name
            )).ToListAsync(cancellationToken);

        return new GetMenuByBoutiqueResult(menus);
    }
}
