using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Purchases;

public record GetPurchaseByBoutiqueResponse(IEnumerable<PurchaseDTO> Purchases);

public class GetPurchaseByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/purchase/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetPurchaseByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetPurchaseByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetPurchaseByBoutique")
       .WithGroupName("Purchases")
       .Produces<BaseResponse<GetPurchaseByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetPurchaseByBoutique By Purchase Id")
       .WithDescription("GetPurchaseByBoutique By Purchase Id")
        .RequireAuthorization();
    }
}
