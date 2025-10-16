using depensio.Application.UseCases.StockLocations.Commands.CreateStockLocation;
using depensio.Application.UseCases.StockLocations.DTOs;
using depensio.Infrastructure.Filters;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.StockLocations;


public record CreateStockLocationRequest(StockLocationDTO StockLocation);
public record CreateStockLocationResponse(Guid Id);

public class CreateStockLocation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/stocklocation/{boutiqueId}", async (Guid boutiqueId, CreateStockLocationRequest request, ISender sender) =>
        {
            var command = new CreateStockLocationCommand(boutiqueId, request.StockLocation);
            var result = await sender.Send(command);

            var response = result.Adapt<CreateStockLocationResponse>();
            var baseResponse = ResponseFactory.Success(response, "StockLocation créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/boutique/{response.Id}", baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("CreateStockLocation")
        .WithGroupName("StockLocations")
        .Produces<BaseResponse<CreateStockLocationResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateStockLocation")
        .WithDescription("CreateStockLocation")
        .RequireAuthorization();
    }
}