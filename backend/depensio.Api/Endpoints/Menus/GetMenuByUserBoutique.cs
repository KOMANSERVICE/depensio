using depensio.Application.UseCases.Menus.DTOs;
using depensio.Application.UseCases.Menus.Queries.GetMenuByUserBoutique;
using depensio.Infrastructure.Filters;

namespace depensio.Api.Endpoints.Menus;

public record GetMenuByUserBoutiqueResponse(IEnumerable<MenuUserDTO> Menus);

public class GetMenuByUserBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/menu/{boutiqueId}/user", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetMenuByUserBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetMenuByUserBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des menus récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetMenuByUserBoutique")
       .WithTags("Menus")
       .Produces<BaseResponse<GetMenuByUserBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("GetMenuByUserBoutique By Menu Id")
       .WithDescription("GetMenuByUserBoutique By Menu Id")
        .RequireAuthorization();
    }
}