
using depensio.Application.UseCases.StockLocations.DTOs;

namespace depensio.Application.UseCases.StockLocations.Queries.GetStockLocationByBoutique;

public class GetStockLocationByBoutiqueHandler(
        IDepensioDbContext _dbContext,
        IUserContextService _contextService
    )
    : IQueryHandler<GetStockLocationByBoutiqueQuery, GetStockLocationByBoutiqueResult>
{
    public async Task<GetStockLocationByBoutiqueResult> Handle(GetStockLocationByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _contextService.GetUserId();

        var stockLocations = await  _dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
            && b.UsersBoutiques.Any(ub => ub.UserId == userId && ub.BoutiqueId == BoutiqueId.Of(request.BoutiqueId)))
            .Include(b => b.StockLocations)
            .SelectMany(b => b.StockLocations)
            .Select(sl => new StockLocationDTO
            {
                Id = sl.Id.Value,
                Name = sl.Name,
                Address = sl.Address
            })
            .ToListAsync();

        return new GetStockLocationByBoutiqueResult(stockLocations);
    }
}
