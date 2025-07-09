using depensio.Application.UserCases.Auth.Commands.VerifyMail;
using depensio.Application.UserCases.Auth.DTOs;

namespace Depensio.API.Endpoints.Auth;

public record VerifyMailRequest(VerifyMailDTO VerifyMail);
public record VerifyMailResponse(bool Result);

public class VerifyMail : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/verifymail", async (VerifyMailRequest request, ISender sender) =>
        {
            var command = request.Adapt<VerifyMailCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<VerifyMailResponse>();

            return Results.Created($"/verifymail/{response.Result}", response);
        })
        .WithName("verifymail")
        .Produces<VerifyMailResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("verifymail")
        .WithDescription("verifymail");
    }
}
