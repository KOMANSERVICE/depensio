using depensio.Application.UseCases.Sales.Commands.CancelSale;

namespace Depensio.Api.Endpoints.Sales;

public record CancelSaleRequest(Guid SaleId, string? Reason);
public record CancelSaleResponse(bool Success);

public class CancelSale : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/sale/cancel", async (CancelSaleRequest request, ISender sender) =>
        {
            var command = request.Adapt<CancelSaleCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CancelSaleResponse>();
            var baseResponse = ResponseFactory.Success(response, "Vente annulée avec succès");

            return Results.Ok(baseResponse);
        })
        .WithName("CancelSale")
        .WithTags("Sales")
        .Produces<BaseResponse<CancelSaleResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Annuler une vente")
        .WithDescription("Annule une vente existante et restaure le stock des produits")
        .RequireAuthorization();
    }
}
