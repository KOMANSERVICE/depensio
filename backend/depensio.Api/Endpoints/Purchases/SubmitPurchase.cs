using depensio.Application.UseCases.Purchases.Commands.SubmitPurchase;

namespace Depensio.Api.Endpoints.Purchases;

public record SubmitPurchaseRequest(Guid BoutiqueId);
public record SubmitPurchaseResponse(Guid Id, string Status);

public class SubmitPurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/purchase/{id:guid}/submit", async (Guid id, SubmitPurchaseRequest request, ISender sender) =>
        {
            var command = new SubmitPurchaseCommand(id, request.BoutiqueId);

            var result = await sender.Send(command);

            var response = result.Adapt<SubmitPurchaseResponse>();
            var baseResponse = ResponseFactory.Success(response, "Achat soumis pour validation avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("SubmitPurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<SubmitPurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Soumettre un achat brouillon pour validation")
        .WithDescription("Permet de soumettre un achat en statut Draft pour approbation. L'achat passe en statut PendingApproval (2). Requiert que PaymentMethod, AccountId et CategoryId soient renseignés, et qu'au moins un article soit présent.")
        .RequireAuthorization();
    }
}
