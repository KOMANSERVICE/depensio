using depensio.Application.UseCases.Sales.DTOs;
using depensio.Application.UseCases.Sales.Queries.GetSaleByBoutique;
using Microsoft.AspNetCore.Mvc;

namespace Depensio.Api.Endpoints.Sales;

public record GetSaleByBoutiqueResponse(IEnumerable<SaleDTO> Sales);

public class GetSaleByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sale/{boutiqueId}", async (Guid boutiqueId, [FromQuery] string? status, ISender sender) =>
        {
            var result = await sender.Send(new GetSaleByBoutiqueQuery(boutiqueId, status));

            var response = result.Adapt<GetSaleByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des ventes récupérées avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetSaleByBoutique")
       .WithTags("Sales")
       .Produces<BaseResponse<GetSaleByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("Récupérer les ventes d'une boutique")
       .WithDescription("Récupère la liste des ventes d'une boutique avec possibilité de filtrer par statut. Paramètre status: validated, cancelled, all. Par défaut: all.")
        .RequireAuthorization();
    }
}
