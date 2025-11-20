using depensio.Application.UseCases.Auth.Queries.SendVerifyMail;

namespace Depensio.API.Endpoints.Auth;

public record SendVerifyMailRequest(string UserId);
public record SendVerifyMailResponse(bool Result);

public class SendVerifyMail : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/sendverifymail", async (SendVerifyMailRequest request, ISender sender) =>
        {
            var query = request.Adapt<SendVerifyMailQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<SendVerifyMailResponse>();

            var baseResponse = ResponseFactory.Success(response, "Mail envoyé avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("SendVerifyMail")
        .WithTags("Login")
        .Produces<BaseResponse<SendVerifyMailResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("SendVerifyMail")
        .WithDescription("SendVerifyMail");
    }
}
