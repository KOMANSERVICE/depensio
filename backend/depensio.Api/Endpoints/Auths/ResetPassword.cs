using depensio.Application.UseCases.Auth.Commands.ResetPassword;
using depensio.Application.UseCases.Auth.DTOs;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Auths;

public record ResetPasswordRequest(ResetPasswordDTO ResetPassword);
public record ResetPasswordResponse(bool Result);

public class ResetPassword : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/resetpassword", async (ResetPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ResetPasswordCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<ResetPasswordResponse>();
            var baseResponse = ResponseFactory.Success(response, "Modification mot de passe avec succès", StatusCodes.Status200OK);

            return Results.Created($"/resetpassword/{response.Result}", baseResponse);
        })
        .WithName("ResetPassword")
        .WithTags("Login")
        .Produces<BaseResponse<ResetPasswordResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("ResetPassword")
        .WithDescription("ResetPassword");
    }
}