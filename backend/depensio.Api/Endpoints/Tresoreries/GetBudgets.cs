using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetBudgets : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/budgets", async (
            Guid boutiqueId,
            [AsParameters] GetBudgetsQueryParams queryParams,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetBudgetsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.IsActive,
                queryParams.StartDate,
                queryParams.EndDate);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des budgets recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetBudgets")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetBudgetsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les budgets")
        .WithDescription("Recupere la liste des budgets pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetBudgetsQueryParams(
    bool? IsActive = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);
