using depensio.Application.UseCases.Sales.DTOs;
using depensio.Application.UseCases.Sales.Queries.GetSaleByBoutique;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Sales;

public record GetSaleByBoutiqueResponse(IEnumerable<SaleDTO> Sales);

public class GetSettingByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sale/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetSaleByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetSaleByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetSaleByBoutique")
       .WithGroupName("Sales")
       .Produces<BaseResponse<GetSaleByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetSaleByBoutique By Sale Id")
       .WithDescription("GetSaleByBoutique By Sale Id")
        .RequireAuthorization();
    }
}
