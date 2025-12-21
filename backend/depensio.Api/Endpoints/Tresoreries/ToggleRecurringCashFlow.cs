using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class ToggleRecurringCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/tresorerie/{boutiqueId}/recurring-cash-flows/{id:guid}/toggle", async (
            Guid boutiqueId,
            Guid id,
            ITresorerieService tresorerieService,
            ILogger<ToggleRecurringCashFlow> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.ToggleRecurringCashFlowAsync(
                id,
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                result.Data!.IsActive
                    ? "Flux de tresorerie recurrent active avec succes"
                    : "Flux de tresorerie recurrent desactive avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("ToggleRecurringCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<ToggleRecurringCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Activer ou desactiver un flux de tresorerie recurrent")
        .WithDescription("Active ou desactive un flux de tresorerie recurrent existant via le microservice Tresorerie.")
        .RequireAuthorization();
    }
}
