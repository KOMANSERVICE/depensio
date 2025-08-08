using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Products.Queries.GetProductByBoutiqueWithStockSetting;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Products;


public record GetProductByBoutiqueWithStockSettingResponse(IEnumerable<ProductDTO> Products);

public class GetProductByBoutiqueWithStockSetting : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/product/{boutiqueId}/stock", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByBoutiqueWithStockSettingQuery(boutiqueId));

            var response = result.Adapt<GetProductByBoutiqueWithStockSettingResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetProductByBoutiqueWithStockSetting")
        .WithGroupName("Produits")
       .Produces<BaseResponse<GetProductByBoutiqueWithStockSettingResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("Obtenir la liste des produits en fonction de l'option de stock")
       .WithDescription("SI l'option qui autorise la vente des produits en rupture est a true, tous les produits seront chargé sinon, seul les produits en stock seront réccuperées")
        .RequireAuthorization();
    }
}
