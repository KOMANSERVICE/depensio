using depensio.Application.UseCases.Menus.DTOs;
using depensio.Application.UseCases.Menus.Queries.GetMenuByBoutique;
using depensio.Infrastructure.Filters;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Menus;

public record GetMenuByBoutiqueResponse(IEnumerable<MenuDTO> Menus);

public class GetMenuByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/menu/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetMenuByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetMenuByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetMenuByBoutique")
       .WithGroupName("Menus")
       .Produces<BaseResponse<GetMenuByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("GetMenuByBoutique By Menu Id")
       .WithDescription("GetMenuByBoutique By Menu Id")
        .RequireAuthorization();
    }
}
