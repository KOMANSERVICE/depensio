using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetPendingCashFlows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/cash-flows/pending", async (
            Guid boutiqueId,
            [AsParameters] GetPendingCashFlowsQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetPendingCashFlows> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetPendingCashFlowsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.Type,
                queryParams.AccountId);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des flux en attente de validation recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetPendingCashFlows")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetPendingCashFlowsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les flux en attente de validation")
        .WithDescription("Recupere la liste des flux de tresorerie en attente de validation pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetPendingCashFlowsQueryParams(
    CashFlowTypeExtended? Type = null,
    Guid? AccountId = null
);
