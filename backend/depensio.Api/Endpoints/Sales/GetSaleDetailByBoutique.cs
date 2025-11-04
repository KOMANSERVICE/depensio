using depensio.Application.UseCases.Sales.DTOs;
using depensio.Application.UseCases.Sales.Queries.GetSaleDetailByBoutique;

namespace Depensio.Api.Endpoints.Sales;


public record GetSaleDetailByBoutiqueResponse(IEnumerable<SaleDetailDTO> SaleDetails);

public class GetSaleDetailByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sale/{boutiqueId}/detail", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetSaleDetailByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetSaleDetailByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetSaleDetailByBoutique")
       .WithTags("Sales")
       .Produces<BaseResponse<GetSaleDetailByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetSaleDetailByBoutique By SaleDetail Id")
       .WithDescription("GetSaleDetailByBoutique By SaleDetail Id")
        .RequireAuthorization();
    }
}

