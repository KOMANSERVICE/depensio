using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class SubmitCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/submit", async (
            Guid boutiqueId,
            Guid cashFlowId,
            ITresorerieService tresorerieService,
            ILogger<SubmitCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.SubmitCashFlowAsync(
                cashFlowId,
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie soumis pour validation avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("SubmitCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<SubmitCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Soumettre un flux de tresorerie pour validation")
        .WithDescription("Soumet un flux de tresorerie en brouillon pour validation via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
