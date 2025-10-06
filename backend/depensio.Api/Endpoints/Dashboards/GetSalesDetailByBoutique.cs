using depensio.Application.UseCases.Dashboard.DTOs;
using depensio.Application.UseCases.Dashboards.Commands.GetSalesDetailByBoutique;
using depensio.Infrastructure.Filters;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Dashboards;


public record GetSalesDetailByBoutiqueRequest(SaleRequestDTO SaleRequest);
public record GetSalesDetailByBoutiqueResponse(IEnumerable<SaleDashboardDTO> SalesDetails);

public class GetSalesDetailByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/dashboard/{boutiqueId}/saledetail", async (Guid boutiqueId, GetSalesDetailByBoutiqueRequest request, ISender sender) =>
        {
            var saleRequest = request.SaleRequest;
            var command = new GetSalesDetailByBoutiqueCommand(boutiqueId, saleRequest);

            var result = await sender.Send(command);

            var response = result.Adapt<GetSalesDetailByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "AssignedProfile créée avec succès", StatusCodes.Status201Created);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetSalesDetailByBoutique")
        .WithGroupName("Dashboard")
        .Produces<BaseResponse<GetSalesDetailByBoutiqueResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("GetSalesDetailByBoutique")
        .WithDescription("GetSalesDetailByBoutique")
        .RequireAuthorization();
    }
}