using depensio.Application.UseCases.Settings.DTOs;
using depensio.Application.UserCases.Boutiques.Commands.UpsetSettingByBoutique;

namespace Depensio.Api.Endpoints.Settings;

public record UpsetSettingByBoutiqueRequest(SettingDTO Setting);
public record UpsetSettingByBoutiqueResponse(Guid Id);

public class UpsetSettingByBoutique : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapPost("/setting", async (UpsetSettingByBoutiqueRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpsetSettingByBoutiqueCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpsetSettingByBoutiqueResponse>();
            var baseResponse = ResponseFactory.Success(response, "Setting créée avec succès", StatusCodes.Status201Created);

            return Results.Created($"/setting/{response.Id}", baseResponse);
        })
        .WithName("UpsetSettingByBoutique")
        .WithTags("Settings")
        .Produces<BaseResponse<UpsetSettingByBoutiqueResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("UpsetSettingByBoutique")
        .WithDescription("UpsetSettingByBoutique")
        .RequireAuthorization();
    }
}
