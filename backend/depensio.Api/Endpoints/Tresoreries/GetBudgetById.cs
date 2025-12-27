using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetBudgetById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/budgets/{budgetId}", async (
            Guid boutiqueId,
            Guid budgetId,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetBudgetByIdAsync(
                budgetId,
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Detail du budget recupere avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetBudgetById")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetBudgetByIdResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Consulter le detail d'un budget")
        .WithDescription("Recupere le detail d'un budget avec ses depenses, repartition par categorie et evolution dans le temps")
        .RequireAuthorization();
    }
}
