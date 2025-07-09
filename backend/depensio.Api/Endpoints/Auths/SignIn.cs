using depensio.Application.UserCases.Auth.DTOs;
using depensio.Application.UserCases.Auth.Queries.SignIn;

namespace Depensio.API.Endpoints.Auth;

public record SignInRequest(SignInDTO Signin);
public record SignInResponse(string Token);

public class SignIn : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/signin", async (SignInRequest request, ISender sender) =>
        {
           ///var signIn = request.SignIn;
            var query = request.Adapt<SignInQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<SignInResponse>();

            return Results.Ok(response);
        })
        .WithName("SignIn")
        .Produces<SignInResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("SignIn")
        .WithDescription("SignIn");
    }
}
