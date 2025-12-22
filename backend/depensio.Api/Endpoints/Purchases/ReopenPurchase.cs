using depensio.Application.UseCases.Purchases.Commands.ReopenPurchase;

namespace Depensio.Api.Endpoints.Purchases;

public record ReopenPurchaseRequest(Guid BoutiqueId);
public record ReopenPurchaseResponse(Guid Id, string Status);

public class ReopenPurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/purchase/{id:guid}/reopen", async (Guid id, ReopenPurchaseRequest request, ISender sender) =>
        {
            var command = new ReopenPurchaseCommand(id, request.BoutiqueId);

            var result = await sender.Send(command);

            var response = result.Adapt<ReopenPurchaseResponse>();
            var baseResponse = ResponseFactory.Success(response, "Achat rouvert avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("ReopenPurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<ReopenPurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Rouvrir un achat rejeté pour correction")
        .WithDescription("Permet de rouvrir un achat en statut Rejected. L'achat passe en statut Draft (1), redevient modifiable, et le motif de rejet précédent est conservé dans l'historique. L'achat pourra ensuite être modifié et resoumis pour validation.")
        .RequireAuthorization();
    }
}
