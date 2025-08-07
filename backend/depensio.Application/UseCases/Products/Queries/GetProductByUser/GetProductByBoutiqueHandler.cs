using depensio.Application.Helpers;
using depensio.Application.Models;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Constants;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using System.Text.Json;

namespace depensio.Application.UseCases.Products.Queries.GetProductByBoutique;


public class GetProductByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IBoutiqueSettingService _settingService,
    IUserContextService _userContextService)
    : IQueryHandler<GetProductByBoutiqueQuery, GetProductByBoutiqueResult>
{
    public async Task<GetProductByBoutiqueResult> Handle(GetProductByBoutiqueQuery request, CancellationToken cancellationToken)
    {

        var userId = _userContextService.GetUserId();

        var config = await _settingService.GetSettingAsync(
            request.BoutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);

        var stockSetting = result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);
        var stockIsAuto = BoolHelper.ToBool(stockSetting.Value.ToString());

        var products = await dbContext.Boutiques
                    .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                                && b.UsersBoutiques.Any(ub => ub.UserId == userId))
                    .Include(b => b.Products)
                        .ThenInclude(p => p.PurchaseItems)
                    .Include(b => b.Products)
                        .ThenInclude(p => p.SaleItems)
                    .SelectMany(b => b.Products)
                    .Select(p => new ProductDTO(
                        p.Id.Value,
                        p.BoutiqueId.Value,
                        p.Name,
                        p.Barcode,
                        p.Price,
                        p.CostPrice,
                        (stockIsAuto
                        ? p.PurchaseItems.Sum(pi => pi.Quantity) - p.SaleItems.Sum(si => si.Quantity) : 
                        p.Stock)
                    ))
                    .ToListAsync();
     

        return new GetProductByBoutiqueResult(products);
    }
}
