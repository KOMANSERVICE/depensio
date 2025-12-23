using depensio.Application.Exceptions;
using depensio.Application.UseCases.Purchases.Commands.TransferPurchase;

namespace Depensio.Api.Endpoints.Purchases;

public record TransferPurchaseRequest(
    Guid BoutiqueId,
    string? PaymentMethod = null,
    Guid? AccountId = null,
    string? CategoryId = null
);
public record TransferPurchaseResponse(Guid Id, string Status, Guid? CashFlowId);

public class TransferPurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/purchase/{id:guid}/transfer", async (Guid id, TransferPurchaseRequest request, ISender sender) =>
        {
            try
            {
                var command = new TransferPurchaseCommand(
                    id,
                    request.BoutiqueId,
                    request.PaymentMethod,
                    request.AccountId,
                    request.CategoryId
                );

                var result = await sender.Send(command);

                var response = result.Adapt<TransferPurchaseResponse>();
                var baseResponse = ResponseFactory.Success(response, "Achat transféré à la trésorerie avec succès", StatusCodes.Status200OK);

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
                    Instance = $"/purchase/{id}/transfer"
                };
                return Results.Problem(problemDetails);
            }
        })
        .WithName("TransferPurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<TransferPurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status502BadGateway)
        .WithSummary("Transférer manuellement un achat approuvé vers la trésorerie")
        .WithDescription("Permet de transférer manuellement un achat approuvé mais non encore transféré vers la trésorerie. Un CashFlow est créé dans le service de trésorerie et le champ IsTransferred est mis à jour.")
        .RequireAuthorization();
    }
}
