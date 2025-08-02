using BuildingBlocks.Responses;
using depensio.Application.UserCases.Boutiques.Commands.CreateBoutique;
using depensio.Application.UserCases.Boutiques.DTOs;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Boutique;

public record CreateBoutiqueRequest(BoutiqueDTO Boutique);
public record CreateBoutiqueResponse(Guid Id);

public class CreateBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/boutique", async (CreateBoutiqueRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateBoutiqueCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Boutique créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/boutique/{response.Id}", baseResponse);
        })
        .WithName("CreateBoutique")
        .WithGroupName("Boutiques")
        .Produces<BaseResponse<CreateBoutiqueResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateBoutique")
        .WithDescription("CreateBoutique")
        .RequireAuthorization();
    }
}