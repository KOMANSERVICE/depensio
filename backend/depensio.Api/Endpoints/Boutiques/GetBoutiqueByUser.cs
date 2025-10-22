using depensio.Application.UserCases.Boutiques.DTOs;
using depensio.Application.UserCases.Boutiques.Queries.GetBoutiqueByUser;
using Depensio.Api.Helpers;

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
            var baseResponse = ResponseFactory.Success(response, "Liste des boutiques réccuperées avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetBoutiqueByUser")
        .WithTags("Boutiques")
       .Produces<BaseResponse<GetBoutiqueByUserResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .WithSummary("GetBoutiqueByUser")
       .WithDescription("GetBoutiqueByUser")
       .RequireAuthorization();
    }
}
