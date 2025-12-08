using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.StockLocations;

public record CreateStockLocationResponse(Guid Id);

public class CreateStockLocation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/stocklocation/{boutiqueId}", async (Guid boutiqueId, CreateStockLocationRequest request, IMagasinService magasinService, ILogger<CreateStockLocation> logger) =>
        {
            try
            {
                var result = await magasinService.CreateMagasinAsync(boutiqueId, request);

                if (!result.Success)
                {
                    return Results.BadRequest(result);
                }

                var response = new CreateStockLocationResponse(result.Data!.Id);
                var baseResponse = ResponseFactory.Success(response, "StockLocation créée avec succès", StatusCodes.Status201Created);

                return Results.Created($"/stocklocation/{boutiqueId}", baseResponse);
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
        .WithName("CreateStockLocation")
        .WithTags("StockLocations")
        .Produces<BaseResponse<CreateStockLocationResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateStockLocation")
        .WithDescription("CreateStockLocation")
        .RequireAuthorization();
    }
}
