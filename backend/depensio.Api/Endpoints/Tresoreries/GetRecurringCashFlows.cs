using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetRecurringCashFlows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/recurring-cash-flows", async (
            Guid boutiqueId,
            [AsParameters] GetRecurringCashFlowsQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetRecurringCashFlows> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetRecurringCashFlowsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.IsActive,
                queryParams.Type);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des flux de tresorerie recurrents recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetRecurringCashFlows")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetRecurringCashFlowsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les flux de tresorerie recurrents")
        .WithDescription("Recupere la liste des flux de tresorerie recurrents pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetRecurringCashFlowsQueryParams(
    bool? IsActive = true,
    CashFlowTypeExtended? Type = null
);
