using depensio.Application.UseCases.Profiles.Commands.AssignedProfileToUser;
using depensio.Application.UseCases.Profiles.DTO;
using depensio.Infrastructure.Filters;

namespace depensio.Api.Endpoints.AssignedProfiles;

public record AssignedProfileToUserRequest(AssigneProfileDTO AssignedProfile);
public record AssignedProfileToUserResponse(Guid Id);

public class AssignedProfileToUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPatch("/profile/{boutiqueId}", async (Guid boutiqueId, AssignedProfileToUserRequest request, ISender sender) =>
        {
            var assignedProfile = request.AssignedProfile;
            var command = new AssignedProfileToUserCommand(boutiqueId, assignedProfile);

            var result = await sender.Send(command);

            var response = result.Adapt<AssignedProfileToUserResponse>();
            var baseResponse = ResponseFactory.Success(response, "AssignedProfile créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/profile/{response.Id}", baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("AssignedAssignedProfileToUser")
        .WithTags("AssignedProfiles")
        .Produces<BaseResponse<AssignedProfileToUserResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("AssignedAssignedProfileToUser")
        .WithDescription("AssignedAssignedProfileToUser")
        .RequireAuthorization();
    }
}