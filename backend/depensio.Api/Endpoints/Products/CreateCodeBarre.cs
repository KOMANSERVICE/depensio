using depensio.Application.UseCases.Products.Commands.CreateCodeBarre;
using depensio.Application.UseCases.Products.DTOs;

namespace Depensio.Api.Endpoints.Products;

public record CreateCodeBarreRequest(ProductItemDTO ProductItem);
public record CreateCodeBarreResponse(BarcodeDTO Barcode);

public class CreateCodeBarre : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/product/generate-barcodes", async (CreateCodeBarreRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateCodeBarreCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateCodeBarreResponse>();
            var baseResponse = ResponseFactory.Success(response, "Code barre générer avec succès", StatusCodes.Status201Created);

            return Results.Ok(baseResponse);
        })
        .WithName("CreateCodeBarre")
        .WithTags("Produits")
        .Produces<BaseResponse<CreateCodeBarreResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateCodeBarre")
        .WithDescription("CreateCodeBarre")
        .RequireAuthorization();
    }
}
