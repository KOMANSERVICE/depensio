using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetCashFlows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/cash-flows", async (
            Guid boutiqueId,
            [AsParameters] GetCashFlowsQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetCashFlows> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetCashFlowsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.Type,
                queryParams.Status,
                queryParams.AccountId,
                queryParams.CategoryId,
                queryParams.StartDate,
                queryParams.EndDate,
                queryParams.Search,
                queryParams.Page,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.SortOrder);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des flux de tresorerie recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetCashFlows")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetCashFlowsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les flux de tresorerie")
        .WithDescription("Recupere la liste paginee des flux de tresorerie pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetCashFlowsQueryParams(
    CashFlowTypeExtended? Type = null,
    CashFlowStatusExtended? Status = null,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "date",
    string SortOrder = "desc"
);
