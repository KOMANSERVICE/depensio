using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.StockLocations;

public record GetStockLocationByIdResponse(StockLocationDTO StockLocation);

public class GetStockLocationById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/stocklocation/{boutiqueId}/{id}", async (Guid boutiqueId, Guid id, IMagasinService magasinService, ILogger<GetStockLocationById> logger) =>
        {
           
            var result = await magasinService.GetMagasinByIdAsync(boutiqueId, id);

            if (!result.Success)
            {
                return Results.BadRequest(result);
            }

            var response = new GetStockLocationByIdResponse(result.Data!.StockLocation);
            var baseResponse = ResponseFactory.Success(response, "Magasin récupéré avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
           
        })
       .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetStockLocationById")
       .WithTags("StockLocations")
       .Produces<BaseResponse<GetStockLocationByIdResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("Récupérer un magasin par son ID")
       .WithDescription("Récupère les détails d'un magasin spécifique par son ID")
       .RequireAuthorization();
    }
}
