using depensio.Application.UseCases.Profiles.DTO;
using depensio.Application.UseCases.Profiles.Queries.GetProfileByBoutique;
using depensio.Infrastructure.Filters;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Profiles;

public record GetProfileByBoutiqueResponse(IEnumerable<ProfileDTO> Profiles);

public class GetProfileByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetProfileByBoutiqueQuery(boutiqueId));

            var response = result.Adapt<GetProfileByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
       .WithName("GetProfileByBoutique")
       .WithTags("Profiles")
       .Produces<BaseResponse<GetProfileByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .ProducesProblem(StatusCodes.Status403Forbidden)
       .WithSummary("GetProfileByBoutique By Profile Id")
       .WithDescription("GetProfileByBoutique By Profile Id")
        .RequireAuthorization();
    }
}