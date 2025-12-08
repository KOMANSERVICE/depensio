using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;

namespace depensio.Api.Endpoints.StockLocations;

public record GetStockLocationByBoutiqueResponse(IEnumerable<StockLocationDTO> StockLocations);

public class GetStockLocationByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/stocklocation/{boutiqueId}", async (Guid boutiqueId, IMagasinService magasinService) =>
        {
            var result = await magasinService.GetMagasinsByBoutiqueAsync(boutiqueId);

            if (!result.Success)
            {
                return Results.BadRequest(result);
            }

            var response = new GetStockLocationByBoutiqueResponse(result.Data!.StockLocations);
            var baseResponse = ResponseFactory.Success(response, "Liste des magasins récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetStockLocationByBoutique")
       .WithTags("StockLocations")
       .Produces<BaseResponse<GetStockLocationByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("GetStockLocationByBoutique By StockLocation Id")
       .WithDescription("GetStockLocationByBoutique By StockLocation Id")
        .RequireAuthorization();
    }
}
