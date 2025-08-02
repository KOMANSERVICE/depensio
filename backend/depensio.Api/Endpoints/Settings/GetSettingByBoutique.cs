using depensio.Application.Models;
using depensio.Application.UseCases.Settings.DTOs;
using depensio.Application.UseCases.Settings.Queries.GetSettingByBoutique;
using Depensio.Api.Helpers;

namespace Depensio.Api.Endpoints.Settings;

public record GetSettingByBoutiqueResponse(string Values);

public class GetSettingByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/setting/{boutiqueId}/{key}", async (Guid boutiqueId, string key, ISender sender) =>
        {
            var result = await sender.Send(new GetSettingByBoutiqueQuery(boutiqueId, key));

            var response = result.Adapt<GetSettingByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Liste des produire récuperés avec succès", StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
       .WithName("GetSettingByBoutique")
       .WithGroupName("Settings")
       .Produces<BaseResponse<GetSettingByBoutiqueResponse>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .ProducesProblem(StatusCodes.Status404NotFound)
       .WithSummary("GetSettingByBoutique By Sale Id")
       .WithDescription("GetSettingByBoutique By Sale Id")
        .RequireAuthorization();
    }
}
