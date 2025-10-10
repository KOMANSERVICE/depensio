using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Queries.GetProductItemByBoutique;


public class GetProductItemByBoutiqueHandler(
    IDepensioDbContext _dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetProductItemByBoutiqueQuery, GetProductItemByBoutiqueResult>
{
    public async Task<GetProductItemByBoutiqueResult> Handle(GetProductItemByBoutiqueQuery request, CancellationToken cancellationToken)
    {

        var userId = _userContextService.GetUserId();
        var products = await _dbContext.Boutiques
                .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                            && b.UsersBoutiques.Any(ub => ub.UserId == userId))
                .SelectMany(b => b.Products)
                .SelectMany(p => p.ProductItems.Where(pi => pi.Status == ProductStatus.Available)
                , (p, pi) => new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductItem = pi
                })
                .GroupBy(x => new { x.ProductId, x.ProductName })
                .Select(g => new ProductBarcodeDTO(
                    g.Key.ProductId.Value,
                    g.Key.ProductName,
                    g.Select(x => new ProductItemBarcodeDTO(x.ProductItem.Id.Value, x.ProductItem.Barcode)).ToList()
                    )
                )
                .OrderBy(p => p.Name)
                .ToListAsync();

        return new GetProductItemByBoutiqueResult(products);
    }
}
