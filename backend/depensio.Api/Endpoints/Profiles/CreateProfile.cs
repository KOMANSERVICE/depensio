using depensio.Application.UseCases.Profiles.Commands.CreateProfile;
using depensio.Application.UseCases.Profiles.DTO;
using depensio.Infrastructure.Filters;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Profiles;

public record CreateProfileRequest(ProfileDTO Profile);
public record CreateProfileResponse(Guid Id);

public class CreateProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/profile/{boutiqueId}", async (Guid boutiqueId, CreateProfileRequest request, ISender sender) =>
        {
            // Affecte le BoutiqueId dans le DTO
            var profileDto = request.Profile;
            var command = new CreateProfileCommand(boutiqueId, profileDto);

            var result = await sender.Send(command);

            var response = result.Adapt<CreateProfileResponse>();
            var baseResponse = ResponseFactory.Success(response, "Profile créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/profile/{response.Id}", baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("CreateProfile")
        .WithTags("Profiles")
        .Produces<BaseResponse<CreateProfileResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("CreateProfile")
        .WithDescription("CreateProfile")
        .RequireAuthorization();
    }
}
