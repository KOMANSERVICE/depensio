using depensio.Application.UseCases.Sales.DTOs;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleByBoutique;

public class GetSaleByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetSaleByBoutiqueQuery, GetSaleByBoutiqueResult>
{
    public async Task<GetSaleByBoutiqueResult> Handle(GetSaleByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        
        var sales = await dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .Include(b => b.Sales)
            .SelectMany(b => b.Sales)
            .Select(p => new SaleDTO
            {
                Id = p.Id.Value,
                BoutiqueId = p.BoutiqueId.Value,
                //Title = p.Title,
                //Description = p.Description
            })
            .ToListAsync();


        return new GetSaleByBoutiqueResult(sales);
    }
}
