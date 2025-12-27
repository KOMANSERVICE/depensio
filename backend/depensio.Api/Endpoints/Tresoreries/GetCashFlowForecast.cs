using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetCashFlowForecast : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/reports/cash-flow-forecast", async (
            Guid boutiqueId,
            int days,
            bool includePending,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetCashFlowForecastAsync(
                applicationId,
                boutiqueId.ToString(),
                days > 0 ? days : 30,
                includePending);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Previsions de tresorerie recuperees avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetCashFlowForecast")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CashFlowForecastDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Previsions de tresorerie")
        .WithDescription("Recupere les previsions de tresorerie pour les prochains jours avec analyse des risques et dates critiques")
        .RequireAuthorization();
    }
}
