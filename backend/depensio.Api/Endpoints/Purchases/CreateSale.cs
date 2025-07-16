using depensio.Application.UseCases.Purchases.Commands.CreatePurchase;
using depensio.Application.UseCases.Purchases.DTOs;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Purchases;


public record CreatePurchaseRequest(PurchaseDTO Purchase);
public record CreatePurchaseResponse(Guid Id);

public class CreatePurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/purchase", async (CreatePurchaseRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreatePurchaseCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreatePurchaseResponse>();
            var baseResponse = ResponseFactory.Success(response, "Purchase créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/purchase/{response.Id}", baseResponse);
        })
        .WithName("CreatePurchase")
        .WithGroupName("Purchases")
        .Produces<BaseResponse<CreatePurchaseResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreatePurchase")
        .WithDescription("CreatePurchase")
        .RequireAuthorization();
    }
}
