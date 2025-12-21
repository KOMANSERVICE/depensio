using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class RejectCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/reject", async (
            Guid boutiqueId,
            Guid cashFlowId,
            RejectCashFlowRequest request,
            ITresorerieService tresorerieService,
            ILogger<RejectCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.RejectCashFlowAsync(
                cashFlowId,
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie rejete avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("RejectCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<RejectCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Rejeter un flux de tresorerie")
        .WithDescription("Rejette un flux de tresorerie en attente de validation via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
