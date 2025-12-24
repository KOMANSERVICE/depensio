

using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Sales.DTOs;
using System.Linq.Expressions;
using System.Text.Json;

namespace depensio.Application.Services;

public class ProductService(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService,
    IBoutiqueSettingService _settingService) : IProductService
{


    private Expression<Func<Product, Product>> productSelector(bool stockIsAuto)
    {
        return p => new Product
        {
            Id = p.Id,
            BoutiqueId = p.BoutiqueId,
            Name = p.Name,
            Barcode = p.Barcode,
            Price = p.Price,
            CostPrice = p.CostPrice,
            Stock = (stockIsAuto
                ? p.PurchaseItems.Where(pi => pi.Purchase.Status == (int)PurchaseStatus.Approved).Sum(pi => pi.Quantity) - p.SaleItems.Where(si => si.Sale.Status != SaleStatus.Cancelled).Sum(si => si.Quantity) :
                p.Stock),
            ProductItems = p.ProductItems.Where(pi => pi.Status == ProductStatus.Available)
            .ToList()
        };
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(Guid boutiqueId)
    {

        var stockIsAuto = await GetStockAuto(boutiqueId);

        var userId = _userContextService.GetUserId();
        var products = dbContext.Boutiques
                   .Where(b => b.Id == BoutiqueId.Of(boutiqueId)
                               && b.UsersBoutiques.Any(ub => ub.UserId == userId))
                   .Include(b => b.Products)
                       .ThenInclude(p => p.PurchaseItems)
                           .ThenInclude(pi => pi.Purchase)
                   .Include(b => b.Products)
                       .ThenInclude(p => p.SaleItems)
                           .ThenInclude(si => si.Sale)
                   .Include(b => b.Products)
                       .ThenInclude(p => p.ProductItems)
                   .SelectMany(b => b.Products)
                   .Select(productSelector(stockIsAuto));

        return products;
    }

    public async Task<Product?> GetOneProductAsync(Guid boutiqueId, Guid productId)
    {

        var stockIsAuto = await GetStockAuto(boutiqueId);

        var userId = _userContextService.GetUserId();
        var product = await dbContext.Boutiques
                   .Where(b => b.Id == BoutiqueId.Of(boutiqueId)
                               && b.UsersBoutiques.Any(ub => ub.UserId == userId))                   
                   .SelectMany(b => b.Products)
                    .Where(p => p.Id == ProductId.Of(productId))
                   .Select(productSelector(stockIsAuto)).FirstOrDefaultAsync();

        return product;
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
