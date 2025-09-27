using depensio.Application.UseCases.Auth.DTOs;
using depensio.Application.UseCases.Auth.Queries.ListeUser;
using depensio.Application.UserCases.Auth.Queries.GetUserInfos;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Auths;

public record GetUserInfosRequest();
public record GetUserInfosResponse(UserInfosDTO UserInfos);

public class GetUserInfos : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/getuserinfos", async (ISender sender) =>
        {
            var result = await sender.Send(new GetUserInfosQuery());

            var response = result.Adapt<GetUserInfosResponse>();
            var baseResponse = ResponseFactory.Success(response, "Information utilisateur récupérée avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("GetUserInfos")
        .WithGroupName("Login")
        .Produces<GetUserInfosResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("GetUserInfos")
        .WithDescription("GetUserInfos")
        .RequireAuthorization();
    }
}