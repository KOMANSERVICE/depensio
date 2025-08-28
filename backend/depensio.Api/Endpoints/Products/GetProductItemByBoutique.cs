using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Products.Queries.GetProductItemByBoutique;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Products;

public record GetProductItemByBoutiqueResponse(IEnumerable<ProductBarcodeDTO> ProductBarcodes);

public class GetProductItemByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/product/{boutiqueId}/barcodes", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetProductItemByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetProductItemByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des code-barres réccuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetProductItemByBoutique")
        .WithGroupName("Produits")
       .Produces<BaseResponse<GetProductItemByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetProductItemByBoutique By Product Id")
       .WithDescription("GetProductItemByBoutique By Product Id")
        .RequireAuthorization();
    }
}
