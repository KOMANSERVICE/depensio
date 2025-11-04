using depensio.Application.UseCases.Menus.DTOs;
using depensio.Application.UseCases.Menus.Queries.GetOneMenuByUserBoutique;
using depensio.Infrastructure.Filters;

namespace depensio.Api.Endpoints.Menus;


public record GetOneMenuByUserBoutiqueResponse(MenuUserDTO Menu);

public class GetOneMenuByUserBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/menu/{boutiqueId}/user/{currentPath}", async (Guid boutiqueId, string currentPath, ISender sender) =>
        {
            var result = await sender.Send(new GetOneMenuByUserBoutiqueQuery(boutiqueId, currentPath));

            var response = result.Adapt<GetOneMenuByUserBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "menu réccuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetOneMenuByUserBoutique")
       .WithTags("Menus")
       .Produces<BaseResponse<GetOneMenuByUserBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("GetOneMenuByUserBoutique By Menu Id")
       .WithDescription("GetOneMenuByUserBoutique By Menu Id")
        .RequireAuthorization();
    }
}