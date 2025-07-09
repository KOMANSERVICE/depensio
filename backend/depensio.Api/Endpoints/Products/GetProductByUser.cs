using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Products.Queries.GetProductByUser;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Products;

public record GetProductByUserResponse(IEnumerable<ProductDTO> Products);

public class GetProductByUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByUserQuery(boutiqueId));

            var response = result.Adapt<GetProductByUserResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetProductByUser")
       .Produces<GetProductByUserResponse>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetProductByUser By Product Id")
       .WithDescription("GetProductByUser By Product Id")
        .RequireAuthorization();
    }
}
