using depensio.Application.UseCases.Sales.DTOs;
using depensio.Application.UseCases.Sales.Queries.GetSaleHistory;
using Microsoft.AspNetCore.Mvc;

namespace Depensio.Api.Endpoints.Sales;

public record GetSaleHistoryResponse(IEnumerable<SaleStatusHistoryDTO> History);

public class GetSaleHistory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sale/{id:guid}/history", async (Guid id, [FromQuery] Guid boutiqueId, ISender sender) =>
        {
            var query = new GetSaleHistoryQuery(id, boutiqueId);

            var result = await sender.Send(query);

            var response = result.Adapt<GetSaleHistoryResponse>();
            var baseResponse = ResponseFactory.Success(response, "Historique des statuts récupéré avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("GetSaleHistory")
        .WithTags("Sales")
        .Produces<BaseResponse<GetSaleHistoryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Consulter l'historique des statuts d'une vente")
        .WithDescription("Retourne la liste chronologique des changements de statut d'une vente. Pour chaque entrée: date, utilisateur, statut avant/après, commentaire. Accessible pour toute vente quel que soit son statut.")
        .RequireAuthorization();
    }
}
