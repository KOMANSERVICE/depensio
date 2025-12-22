using depensio.Application.Exceptions;
using depensio.Application.UseCases.Purchases.Commands.CancelPurchase;

namespace Depensio.Api.Endpoints.Purchases;

public record CancelPurchaseRequest(Guid BoutiqueId, string? Reason);
public record CancelPurchaseResponse(Guid Id, string Status);

public class CancelPurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/purchase/{id:guid}/cancel", async (Guid id, CancelPurchaseRequest request, ISender sender) =>
        {
            try
            {
                var command = new CancelPurchaseCommand(id, request.BoutiqueId, request.Reason);

                var result = await sender.Send(command);

                var response = result.Adapt<CancelPurchaseResponse>();
                var baseResponse = ResponseFactory.Success(response, "Achat annulé avec succès", StatusCodes.Status200OK);

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
                    Instance = $"/purchase/{id}/cancel"
                };
                return Results.Problem(problemDetails);
            }
        })
        .WithName("CancelPurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<CancelPurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status502BadGateway)
        .WithSummary("Annuler un achat")
        .WithDescription("Permet d'annuler un achat selon son statut actuel:\n" +
            "- US-PUR-008 (Approved → Cancelled): Motif obligatoire, suppression du mouvement de trésorerie associé si présent.\n" +
            "- US-PUR-009 (Draft/PendingApproval/Rejected → Cancelled): Motif optionnel, pas d'appel à la trésorerie.\n" +
            "L'achat passe en statut Cancelled (5) et l'historique est créé. Cet état est final, aucune transition n'est possible depuis Cancelled.")
        .RequireAuthorization();
    }
}
