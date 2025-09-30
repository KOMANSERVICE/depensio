using depensio.Application.UseCases.Auth.Commands.ForgetPassword;
using depensio.Application.UseCases.Auth.DTOs;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Auths;

public record ForgotPasswordRequest(ForgotPasswordDTO ForgotPassword);
public record ForgotPasswordResponse(bool Result);

public class ForgotPassword : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/forgotpassword", async (ForgotPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ForgotPasswordCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<ForgotPasswordResponse>();
            var baseResponse = ResponseFactory.Success(response, "Modification mot de passe avec succès", StatusCodes.Status200OK);

            return Results.Created($"/forgotpassword/{response.Result}", baseResponse);
        })
        .WithName("ForgotPassword")
        .WithGroupName("Login")
        .Produces<BaseResponse<ForgotPasswordResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("ForgotPassword")
        .WithDescription("ForgotPassword");
    }
}

