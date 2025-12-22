using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Application.UseCases.Purchases.Queries.GetPurchaseHistory;
using Microsoft.AspNetCore.Mvc;

namespace Depensio.Api.Endpoints.Purchases;

public record GetPurchaseHistoryResponse(IEnumerable<PurchaseStatusHistoryDTO> History);

public class GetPurchaseHistory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/purchase/{id:guid}/history", async (Guid id, [FromQuery] Guid boutiqueId, ISender sender) =>
        {
            var query = new GetPurchaseHistoryQuery(id, boutiqueId);

            var result = await sender.Send(query);

            var response = result.Adapt<GetPurchaseHistoryResponse>();
            var baseResponse = ResponseFactory.Success(response, "Historique des statuts récupéré avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("GetPurchaseHistory")
        .WithTags("Purchases")
        .Produces<BaseResponse<GetPurchaseHistoryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Consulter l'historique des statuts d'un achat")
        .WithDescription("Retourne la liste chronologique des changements de statut d'un achat. Pour chaque entrée: date, utilisateur, statut avant/après, commentaire. Accessible pour tout achat quel que soit son statut.")
        .RequireAuthorization();
    }
}
