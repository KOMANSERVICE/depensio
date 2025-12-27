using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetCashFlowStatement : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/reports/cash-flow-statement", async (
            Guid boutiqueId,
            DateTime? startDate,
            DateTime? endDate,
            bool comparePrevious,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetCashFlowStatementAsync(
                applicationId,
                boutiqueId.ToString(),
                startDate,
                endDate,
                comparePrevious);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Etat des flux de tresorerie recupere avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetCashFlowStatement")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetCashFlowStatementResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Etat des flux de tresorerie")
        .WithDescription("Recupere l'etat des flux de tresorerie avec repartition par categorie et comparaison avec la periode precedente")
        .RequireAuthorization();
    }
}
