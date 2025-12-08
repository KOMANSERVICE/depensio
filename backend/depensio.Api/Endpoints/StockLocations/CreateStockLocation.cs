using depensio.Application.ApiExterne.Magasins;
using depensio.Infrastructure.Filters;

namespace depensio.Api.Endpoints.StockLocations;

public record CreateStockLocationRequest(StockLocationCreateDTO StockLocation);
public record CreateStockLocationResponse(Guid Id);

public class CreateStockLocation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/stocklocation/{boutiqueId}", async (Guid boutiqueId, CreateStockLocationRequest request, IMagasinService magasinService) =>
        {
            var result = await magasinService.CreateMagasinAsync(boutiqueId, request.StockLocation);

            if (!result.Success)
            {
                return Results.BadRequest(result);
            }

            var response = new CreateStockLocationResponse(result.Data!.Id);
            var baseResponse = ResponseFactory.Success(response, "StockLocation créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/stocklocation/{boutiqueId}", baseResponse);
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
