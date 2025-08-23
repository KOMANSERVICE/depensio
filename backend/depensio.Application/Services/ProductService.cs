

using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Sales.DTOs;
using System.Text.Json;

namespace depensio.Application.Services;

public class ProductService(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService,
    IBoutiqueSettingService _settingService) : IProductService
{
    public async Task<IEnumerable<Product>> GetProductsAsync(Guid boutiqueId)
    {

        var stockIsAuto = await GetStockAuto(boutiqueId);

        var userId = _userContextService.GetUserId();
        var products = dbContext.Boutiques
                   .Where(b => b.Id == BoutiqueId.Of(boutiqueId)
                               && b.UsersBoutiques.Any(ub => ub.UserId == userId))
                   .Include(b => b.Products)
                       .ThenInclude(p => p.PurchaseItems)
                   .Include(b => b.Products)
                       .ThenInclude(p => p.SaleItems)
                   .SelectMany(b => b.Products)
                   .Select(p => new Product
                   {
                        Id = p.Id,
                        BoutiqueId = p.BoutiqueId,
                        Name = p.Name,
                        Barcode = p.Barcode,
                        Price = p.Price,
                        CostPrice = p.CostPrice,
                        Stock = (stockIsAuto
                            ? p.PurchaseItems.Sum(pi => pi.Quantity) - p.SaleItems.Sum(si => si.Quantity) :
                            p.Stock)
                   }
                   );

        return products;
    }


    private async Task<bool> GetStockAuto(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );
        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);
        var stockSetting = result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);
        return BoolHelper.ToBool(stockSetting?.Value.ToString());
    }
}
