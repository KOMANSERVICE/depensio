using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}", async (
            Guid boutiqueId,
            Guid cashFlowId,
            ITresorerieService tresorerieService,
            ILogger<GetCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.GetCashFlowAsync(
                cashFlowId,
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new NotFoundException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie recupere avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Recuperer un flux de tresorerie")
        .WithDescription("Recupere les details d'un flux de tresorerie pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
