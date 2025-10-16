using depensio.Application.UseCases.StockLocations.DTOs;
using depensio.Application.UseCases.StockLocations.Queries.GetStockLocationByBoutique;
using depensio.Infrastructure.Filters;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.StockLocations;

public record GetStockLocationByBoutiqueResponse(IEnumerable<StockLocationDTO> StockLocations);

public class GetStockLocationByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/stocklocation/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetStockLocationByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetStockLocationByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des magasins récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetStockLocationByBoutique")
       .WithGroupName("StockLocations")
       .Produces<BaseResponse<GetStockLocationByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("GetStockLocationByBoutique By StockLocation Id")
       .WithDescription("GetStockLocationByBoutique By StockLocation Id")
        .RequireAuthorization();
    }
}
