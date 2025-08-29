using depensio.Application.Helpers;
using depensio.Application.Models;
using depensio.Application.Services;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Constants;
using System.Text.Json;

namespace depensio.Application.UseCases.Products.Queries.GetProductByBoutique;


public class GetProductByBoutiqueHandler(
    IProductService _productService)
    : IQueryHandler<GetProductByBoutiqueQuery, GetProductByBoutiqueResult>
{
    public async Task<GetProductByBoutiqueResult> Handle(GetProductByBoutiqueQuery request, CancellationToken cancellationToken)
    {

        var products = await _productService.GetProductsAsync(request.BoutiqueId);  
        var result =  products.Select(p => new ProductDTO(
                p.Id.Value,
                p.BoutiqueId.Value,
                p.Name,
                p.Barcode,
                p.Price,
                p.CostPrice,
                p.Stock,
                p.ProductItems.Select(pi => pi.Barcode).ToList()
            )).ToList();



        return new GetProductByBoutiqueResult(result);
    }
}
