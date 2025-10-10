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
    IBoutiqueSettingService _settingService,
    IProductService _productService)
    : IQueryHandler<GetProductByBoutiqueWithStockSettingQuery, GetProductByBoutiqueWithStockSettingResult>
{
    public async Task<GetProductByBoutiqueWithStockSettingResult> Handle(GetProductByBoutiqueWithStockSettingQuery request, CancellationToken cancellationToken)
    {
                      
        var productZeroStock = await AutoriserLesProduitAvecStockZero(request.BoutiqueId); // à ajouter dans ton DTO de requête

        var productsQuery = await _productService.GetProductsAsync(request.BoutiqueId);
         
        if (!productZeroStock)
        {
            productsQuery = productsQuery.Where(p => p.Stock > 0);
        }

        var products = productsQuery
            .Select(p => new ProductDTO(
                p.Id.Value,
                p.BoutiqueId.Value,
                p.Name,
                p.Barcode,
                p.Price,
                p.CostPrice,
                p.Stock,
                p.ProductItems.Select(pi => pi.Barcode).ToList()
            ))
            .OrderBy(p => p.Name)
            .ToList();


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
