using depensio.Application.UseCases.Boutiques.Queries.GetOneBoutiqueByUser;
using depensio.Application.UserCases.Boutiques.DTOs;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Boutiques;

public record GetOneBoutiqueByUserResponse(BoutiqueDTO Boutique);

public class GetOneBoutiqueByUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/boutique/user/{boutiqueId}", async (Guid boutiqueId,ISender sender) =>
        {
            var result = await sender.Send(new GetOneBoutiqueByUserQuery(boutiqueId));

            var response = result.Adapt<GetOneBoutiqueByUserResponse>();
            var baseResponse = ResponseFactory.Success(response, "Boutique réccuperé avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetOneBoutiqueByUser")
        .WithTags("Boutiques")
       .Produces<BaseResponse<GetOneBoutiqueByUserResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .WithSummary("GetOneBoutiqueByUser")
       .WithDescription("GetOneBoutiqueByUser")
       .RequireAuthorization();
    }
}

