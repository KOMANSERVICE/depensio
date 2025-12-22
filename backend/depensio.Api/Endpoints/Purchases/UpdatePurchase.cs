using depensio.Application.UseCases.Purchases.Commands.UpdatePurchase;
using depensio.Application.UseCases.Purchases.DTOs;
using IDR.Library.BuildingBlocks.Exceptions;

namespace Depensio.Api.Endpoints.Purchases;

public record UpdatePurchaseRequest(PurchaseDTO Purchase);
public record UpdatePurchaseResponse(Guid Id);

public class UpdatePurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/purchase/{id:guid}", async (Guid id, UpdatePurchaseRequest request, ISender sender) =>
        {
            // Ensure the ID in the route matches the ID in the request body
            if (id != request.Purchase.Id)
            {
                throw new BadRequestException("L'identifiant de l'achat ne correspond pas.");
            }

            var command = request.Adapt<UpdatePurchaseCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdatePurchaseResponse>();
            var baseResponse = ResponseFactory.Success(response, "Achat modifié avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("UpdatePurchase")
        .WithTags("Purchases")
        .Produces<BaseResponse<UpdatePurchaseResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Modifier un achat en brouillon ou rejeté")
        .WithDescription("Permet de modifier un achat qui est en statut Draft ou Rejected. Les achats Approved, Pending ou Cancelled ne peuvent pas être modifiés.")
        .RequireAuthorization();
    }
}
