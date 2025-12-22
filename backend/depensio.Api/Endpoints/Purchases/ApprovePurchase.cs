using depensio.Application.Exceptions;
using depensio.Application.UseCases.Purchases.Commands.ApprovePurchase;

namespace Depensio.Api.Endpoints.Purchases;

public record ApprovePurchaseRequest(Guid BoutiqueId);
public record ApprovePurchaseResponse(Guid Id, string Status, Guid? CashFlowId);

public class ApprovePurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/purchase/{id:guid}/approve", async (Guid id, ApprovePurchaseRequest request, ISender sender) =>
        {
            try
            {
                var command = new ApprovePurchaseCommand(id, request.BoutiqueId);

                var result = await sender.Send(command);

                var response = result.Adapt<ApprovePurchaseResponse>();
                var baseResponse = ResponseFactory.Success(response, "Achat approuvé avec succès", StatusCodes.Status200OK);

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
                    Instance = $"/purchase/{id}/approve"
                };
                return Results.Problem(problemDetails);
            }
        })
        .WithName("ApprovePurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<ApprovePurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status502BadGateway)
        .WithSummary("Approuver un achat en attente de validation")
        .WithDescription("Permet d'approuver un achat en statut PendingApproval. L'achat passe en statut Approved (3), un CashFlow est créé dans le service de trésorerie, et les champs ApprovedAt, ApprovedBy et CashFlowId sont renseignés.")
        .RequireAuthorization();
    }
}
