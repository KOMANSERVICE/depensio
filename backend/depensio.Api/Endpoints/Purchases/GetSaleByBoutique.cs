using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;
using Microsoft.AspNetCore.Mvc;

namespace Depensio.Api.Endpoints.Purchases;

public record GetPurchaseByBoutiqueResponse(IEnumerable<PurchaseDTO> Purchases);

public class GetPurchaseByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/purchase/{boutiqueId}", async (Guid boutiqueId, [FromQuery] string? status, ISender sender) =>
        {
            var result = await sender.Send(new GetPurchaseByBoutiqueQuery(boutiqueId, status));

            var response = result.Adapt<GetPurchaseByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des achats récupérés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetPurchaseByBoutique")
       .WithTags("Purchases")
       .Produces<BaseResponse<GetPurchaseByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("Récupérer les achats d'une boutique")
       .WithDescription("Récupère la liste des achats d'une boutique avec possibilité de filtrer par statut. Paramètre status: draft, pending, approved, rejected, cancelled, all. Plusieurs statuts peuvent être combinés avec une virgule (ex: status=draft,pending). Par défaut: all.")
        .RequireAuthorization();
    }
}
