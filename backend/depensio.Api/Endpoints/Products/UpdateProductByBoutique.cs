using depensio.Application.UseCases.Products.Commands.UpdateProductByBoutique;
using depensio.Application.UseCases.Products.DTOs;

namespace Depensio.Api.Endpoints.Products;

public record UpdateProductByBoutiqueRequest(ProductUpdateDTO Product);
public record UpdateProductByBoutiqueResponse(Guid Id);

public class UpdateProductByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/product", async (UpdateProductByBoutiqueRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateProductByBoutiqueCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateProductByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Product mise à jour avec succès", StatusCodes.Status200OK);

            return Results.Created($"/product/{response.Id}", baseResponse);
        })
        .WithName("UpdateProductByBoutique")
        .WithTags("Produits")
        .Produces<BaseResponse<UpdateProductByBoutiqueResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("UpdateProductByBoutique")
        .WithDescription("UpdateProductByBoutique")
        .RequireAuthorization();
    }
}

