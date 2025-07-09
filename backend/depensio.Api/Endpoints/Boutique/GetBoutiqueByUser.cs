using depensio.Application.Boutiques.DTOs;
using depensio.Application.Boutiques.Queries.GetBoutiqueByUser;

namespace Depensio.Api.Endpoints.Boutique;

public record GetBoutiqueByUserResponse(IEnumerable<BoutiqueDTO> Boutiques);

public class GetBoutiqueByUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/boutique/user", async (ISender sender) =>
        {
            var result = await sender.Send(new GetBoutiqueByUserQuery());

            var response = result.Adapt<GetBoutiqueByUserResponse>();

            return Results.Ok(response);
        })
       .WithName("GetBoutiqueByUser")
       .Produces<GetBoutiqueByUserResponse>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .WithSummary("GetBoutiqueByUser")
       .WithDescription("GetBoutiqueByUser")
       .RequireAuthorization();
    }
}
