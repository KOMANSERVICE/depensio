using depensio.Application.UseCases.Sales.DTOs;
using depensio.Application.UseCases.Sales.Queries.GetSaleSummaryByBoutique;

namespace Depensio.Api.Endpoints.Sales;


public record GetSaleSummaryByBoutiqueResponse(IEnumerable<SaleSummaryDTO> SaleSummarys);

public class GetSaleSummaryByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sale/{boutiqueId}/summary", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetSaleSummaryByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetSaleSummaryByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetSaleSummaryByBoutique")
       .WithTags("Sales")
       .Produces<BaseResponse<GetSaleSummaryByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetSaleSummaryByBoutique By SaleSummary Id")
       .WithDescription("GetSaleSummaryByBoutique By SaleSummary Id")
        .RequireAuthorization();
    }
}

