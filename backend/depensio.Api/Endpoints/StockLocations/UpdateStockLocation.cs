using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
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
            
            var result = await magasinService.UpdateMagasinAsync(boutiqueId, stockLocationId, request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var response = new UpdateStockLocationResponse(result.Data!.Id);
            var baseResponse = ResponseFactory.Success(response, "Magasin modifié avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
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
