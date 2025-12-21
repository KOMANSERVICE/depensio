using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetUnreconciledCashFlows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/cash-flows/unreconciled", async (
            Guid boutiqueId,
            [AsParameters] GetUnreconciledCashFlowsQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetUnreconciledCashFlows> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetUnreconciledCashFlowsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.AccountId,
                queryParams.StartDate,
                queryParams.EndDate);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des flux non reconcilies recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetUnreconciledCashFlows")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetUnreconciledCashFlowsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les flux non reconcilies")
        .WithDescription("Recupere la liste des flux de tresorerie non reconcilies pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetUnreconciledCashFlowsQueryParams(
    Guid? AccountId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);
