using depensio.Application.UseCases.Auth.Queries.ListeUser;
using depensio.Application.UseCases.Products.Queries.GetProductByBoutique;
using depensio.Domain.ValueObjects;
using Depensio.Api.Helpers;

namespace depensio.Api.Endpoints.Auths;

public record GetListeUserRequest(Guid BoutiqueId);
public record GetListeUserResponse(IEnumerable<SignUpBoutiqueDTO> ListeUsers);

public class GetListeUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/getlisteuser/{boutiqueId}", async (Guid boutiqueId, ISender sender) =>
        {
            var result = await sender.Send(new GetListeUserQuery(boutiqueId));

            var response = result.Adapt<GetListeUserResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des utilisateurs récupérée avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("GetListeUser")
        .WithGroupName("Login")
        .Produces<GetListeUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("GetListeUser")
        .WithDescription("GetListeUser")
        .RequireAuthorization();
    }
}