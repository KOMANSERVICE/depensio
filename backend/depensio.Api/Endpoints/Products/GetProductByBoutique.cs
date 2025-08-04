using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Products.Queries.GetProductByBoutique;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Products;

public record GetProductByBoutiqueResponse(IEnumerable<ProductDTO> Products);

public class GetProductByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/product/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetProductByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetProductByBoutique")
        .WithGroupName("Produits")
       .Produces<BaseResponse<GetProductByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetProductByBoutique By Product Id")
       .WithDescription("GetProductByBoutique By Product Id")
        .RequireAuthorization();
    }
}
