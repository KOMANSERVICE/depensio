using depensio.Application.Helpers;
using depensio.Application.Models;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Constants;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using MediatR;
using System.Text.Json;

namespace depensio.Application.UseCases.Products.Queries.GetProductByBoutiqueWithStockSetting;


public class GetProductByBoutiqueWithStockSettingHandler(
    IDepensioDbContext dbContext,
    IBoutiqueSettingService _settingService,
    IUserContextService _userContextService)
    : IQueryHandler<GetProductByBoutiqueWithStockSettingQuery, GetProductByBoutiqueWithStockSettingResult>
{
    public async Task<GetProductByBoutiqueWithStockSettingResult> Handle(GetProductByBoutiqueWithStockSettingQuery request, CancellationToken cancellationToken)
    {

        var userId = _userContextService.GetUserId();

        var config = await _settingService.GetSettingAsync(
            request.BoutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);

        var stockSetting = result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);
        var stockIsAuto = BoolHelper.ToBool(stockSetting.Value.ToString());
              
        var productZeroStock = await AutoriserLesProduitAvecStockZero(request.BoutiqueId); // à ajouter dans ton DTO de requête

        var productsQuery = dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .Include(b => b.Products)
                .ThenInclude(p => p.PurchaseItems)
            .Include(b => b.Products)
                .ThenInclude(p => p.SaleItems)
            .SelectMany(b => b.Products)
            .Select(p => new
            {
                Product = p,
                CalculatedStock = stockIsAuto
                    ? p.PurchaseItems.Sum(pi => pi.Quantity) - p.SaleItems.Sum(si => si.Quantity)
                    : p.Stock
            });

        if (!productZeroStock)
        {
            productsQuery = productsQuery.Where(p => p.CalculatedStock > 0);
        }

        var products = await productsQuery
            .Select(p => new ProductDTO(
                p.Product.Id.Value,
                p.Product.BoutiqueId.Value,
                p.Product.Name,
                p.Product.Barcode,
                p.Product.Price,
                p.Product.CostPrice,
                p.CalculatedStock
            ))
            .ToListAsync();


        return new GetProductByBoutiqueWithStockSettingResult(products);
    }

    private async Task<bool> AutoriserLesProduitAvecStockZero(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
                  boutiqueId,
                  BoutiqueSettingKeys.VENTE_KEY
              );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);

        var autoVenteStockZero = result?.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.VENTE_AUTORISER_VENTE_AVEC_STOCK_ZERO);
       
        return BoolHelper.ToBool(autoVenteStockZero?.Value.ToString());
    }

}
