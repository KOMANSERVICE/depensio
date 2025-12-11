using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.StockLocations;

public record UpdateStockLocationResponse(Guid Id);

public class UpdateStockLocation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/stocklocation/{boutiqueId}/{stockLocationId}", async (Guid boutiqueId, Guid stockLocationId, UpdateStockLocationRequest request, IMagasinService magasinService, ILogger<UpdateStockLocation> logger) =>
        {
            try
            {
                var result = await magasinService.UpdateMagasinAsync(boutiqueId, stockLocationId, request);

                if (!result.Success)
                {
                    return Results.BadRequest(result);
                }

                var response = new UpdateStockLocationResponse(result.Data!.Id);
                var baseResponse = ResponseFactory.Success(response, "Magasin modifié avec succès", StatusCodes.Status200OK);

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
        .WithName("UpdateStockLocation")
        .WithTags("StockLocations")
        .Produces<BaseResponse<UpdateStockLocationResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Modifier un magasin")
        .WithDescription("Modifie le nom et l'adresse d'un magasin existant")
        .RequireAuthorization();
    }
}
