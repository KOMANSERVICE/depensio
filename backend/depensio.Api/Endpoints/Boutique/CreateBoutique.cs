

using depensio.Application.Boutiques.Commands.CreateBoutique;
using depensio.Application.Boutiques.DTOs;

namespace Depensio.Api.Endpoints.Boutique;


public record CreateBoutiqueRequest(BoutiqueDTO Boutique);
public record CreateBoutiqueResponse(Guid Id);

public class CreateBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/CreateBoutique", async (CreateBoutiqueRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateBoutiqueCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateBoutiqueResponse>();

            return Results.Created($"/CreateBoutique/{response.Id}", response);
        })
        .WithName("CreateBoutique")
        .Produces<CreateBoutiqueResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateBoutique")
        .WithDescription("CreateBoutique")
        .RequireAuthorization();
    }
}