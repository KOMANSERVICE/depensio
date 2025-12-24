using depensio.Application.Exceptions;
using depensio.Application.UseCases.Sales.Commands.TransferSale;

namespace Depensio.Api.Endpoints.Sales;

public record TransferSaleRequest(
    Guid BoutiqueId,
    string? PaymentMethod = null,
    Guid? AccountId = null,
    string? CategoryId = null
);
public record TransferSaleResponse(Guid Id, string Status, Guid? CashFlowId);

public class TransferSale : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/sale/{id:guid}/transfer", async (Guid id, TransferSaleRequest request, ISender sender) =>
        {
            try
            {
                var command = new TransferSaleCommand(
                    id,
                    request.BoutiqueId,
                    request.PaymentMethod,
                    request.AccountId,
                    request.CategoryId
                );

                var result = await sender.Send(command);

                var response = result.Adapt<TransferSaleResponse>();
                var baseResponse = ResponseFactory.Success(response, "Vente transférée à la trésorerie avec succès", StatusCodes.Status200OK);

                return Results.Ok(baseResponse);
            }
            catch (ExternalServiceException ex)
            {
                // Return 502 Bad Gateway for external service failures
                var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = StatusCodes.Status502BadGateway,
                    Title = "Erreur du service externe",
                    Detail = ex.Message,
                    Instance = $"/sale/{id}/transfer"
                };
                return Results.Problem(problemDetails);
            }
        })
        .WithName("TransferSale")
        .WithTags("Sales")
        .Produces<BaseResponse<TransferSaleResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status502BadGateway)
        .WithSummary("Transférer manuellement une vente validée vers la trésorerie")
        .WithDescription("Permet de transférer manuellement une vente validée mais non encore transférée vers la trésorerie. Un CashFlow est créé dans le service de trésorerie.")
        .RequireAuthorization();
    }
}
