using depensio.Application.UseCases.Sales.Commands.CreateSale;
using depensio.Application.UseCases.Sales.DTOs;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Sales;


public record CreateSaleRequest(SaleDTO Sale);
public record CreateSaleResponse(Guid Id);

public class CreateSale : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/sale", async (CreateSaleRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateSaleCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateSaleResponse>();
            var baseResponse = ResponseFactory.Success(response, "Sale créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/sale/{response.Id}", baseResponse);
        })
        .WithName("CreateSale")
        .WithTags("Sales")
        .Produces<BaseResponse<CreateSaleResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateSale")
        .WithDescription("CreateSale")
        .RequireAuthorization();
    }
}
