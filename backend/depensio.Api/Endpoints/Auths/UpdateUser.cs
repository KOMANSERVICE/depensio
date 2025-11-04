using depensio.Application.UseCases.Auth.Commands.UpdateUser;
using depensio.Application.UseCases.Auth.DTOs;

namespace depensio.Api.Endpoints.Auths;

public record UpdateUserRequest(UserInfosDTO UserInfos);
public record UpdateUserResponse(bool Result);

public class UpdateUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/auth/updateuser", async (UpdateUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateUserResponse>();
            var baseResponse = ResponseFactory.Success(response, "Product mise à jour avec succès", StatusCodes.Status200OK);

            return Results.Created($"/updateuser/{response}", baseResponse);
        })
        .WithName("UpdateUser")
        .WithTags("Login")
        .Produces<BaseResponse<UpdateUserResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("UpdateUser")
        .WithDescription("UpdateUser")
        .RequireAuthorization();
    }
}

