using depensio.Application.UseCases.Purchases.Commands.RejectPurchase;

namespace Depensio.Api.Endpoints.Purchases;

public record RejectPurchaseRequest(Guid BoutiqueId, string Reason);
public record RejectPurchaseResponse(Guid Id, string Status);

public class RejectPurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/purchase/{id:guid}/reject", async (Guid id, RejectPurchaseRequest request, ISender sender) =>
        {
            var command = new RejectPurchaseCommand(id, request.BoutiqueId, request.Reason);

            var result = await sender.Send(command);

            var response = result.Adapt<RejectPurchaseResponse>();
            var baseResponse = ResponseFactory.Success(response, "Achat rejeté avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("RejectPurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<RejectPurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Rejeter un achat en attente de validation")
        .WithDescription("Permet de rejeter un achat en statut PendingApproval. L'achat passe en statut Rejected (4), le motif de rejet est enregistré, et l'historique est créé. L'achat pourra ensuite être modifié et resoumis.")
        .RequireAuthorization();
    }
}
