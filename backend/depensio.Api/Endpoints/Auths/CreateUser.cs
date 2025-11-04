using depensio.Application.UseCases.Auth.Commands.CreateUser;

namespace depensio.Api.Endpoints.Auths;

public record CreateUserRequest(SignUpBoutiqueDTO Signup);
public record CreateUserResponse(bool Result);

public class CreateUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/createuser", async (CreateUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateUserResponse>();
            var baseResponse = ResponseFactory.Success(response, "Utilisateur créer avec succès", StatusCodes.Status201Created);
            
            return Results.Created($"/createuser/{response.Result}", baseResponse);
        })
        .WithName("CreateUser")
        .WithTags("Login")
        .Produces<BaseResponse<CreateUserResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("CreateUser")
        .WithDescription("CreateUser")
        .RequireAuthorization();
    }
}
