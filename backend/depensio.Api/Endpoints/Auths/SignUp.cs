using depensio.Application.UserCases.Auth.Commands.SignUp;

namespace Depensio.API.Endpoints.Auth;

public record SignUpRequest(SignUpDTO Signup);
public record SignUpResponse(bool Result);

public class SignUp : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/signup",async (SignUpRequest request, ISender sender) =>
        {
            var command = request.Adapt<SignUpCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<SignUpResponse>();

            return Results.Created($"/signup/{response.Result}", response);
        })
        .WithName("SignUp")
        .WithTags("Login")
        .Produces<SignUpResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("SignUp")
        .WithDescription("SignUp");
    }
}

