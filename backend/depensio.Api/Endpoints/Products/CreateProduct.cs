using depensio.Application.UseCases.Products.Commands.CreateProduct;
using depensio.Application.UseCases.Products.DTOs;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Products;

public record CreateProductRequest(ProductDTO Product);
public record CreateProductResponse(Guid Id);

public class CreateProduct : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/product", async (CreateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateProductCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateProductResponse>();
            var baseResponse = ResponseFactory.Success(response, "Product créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/product/{response.Id}", baseResponse);
        })
        .WithName("CreateProduct")
        .Produces<BaseResponse<CreateProductResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateProduct")
        .WithDescription("CreateProduct")
        .RequireAuthorization();
    }
}
