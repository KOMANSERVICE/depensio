using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.StockLocations;

public record GetStockLocationByBoutiqueResponse(IEnumerable<StockLocationDTO> StockLocations);

public class GetStockLocationByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/stocklocation/{boutiqueId}", async (Guid boutiqueId, IMagasinService magasinService, ILogger<GetStockLocationByBoutique> logger) =>
        {
            try
            {
                var result = await magasinService.GetMagasinsByBoutiqueAsync(boutiqueId);

                if (!result.Success)
                {
                    return Results.BadRequest(result);
                }

                var response = new GetStockLocationByBoutiqueResponse(result.Data!.StockLocations);
                var baseResponse = ResponseFactory.Success(response, "Liste des magasins récuperés avec succès", StatusCodes.Status200OK);

                return Results.Ok(baseResponse);
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, "Erreur lors de l'appel au microservice Magasin: {StatusCode} - {Content}", ex.StatusCode, ex.Content);
                return Results.Problem(
                    detail: ex.Content ?? "Erreur interne du service Magasin",
                    statusCode: (int)ex.StatusCode,
                    title: "Erreur du microservice Magasin");
            }
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
