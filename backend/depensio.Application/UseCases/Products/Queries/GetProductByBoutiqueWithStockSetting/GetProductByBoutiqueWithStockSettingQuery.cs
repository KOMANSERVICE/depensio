using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Queries.GetProductByBoutiqueWithStockSetting;

public record GetProductByBoutiqueWithStockSettingQuery(Guid BoutiqueId)
    : IQuery<GetProductByBoutiqueWithStockSettingResult>;
public record GetProductByBoutiqueWithStockSettingResult(IEnumerable<ProductDTO> Products);