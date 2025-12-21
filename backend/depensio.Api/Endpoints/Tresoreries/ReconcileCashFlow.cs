using depensio.Application.ApiExterne.Tresoreries;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class ReconcileCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/reconcile", async (
            Guid boutiqueId,
            Guid cashFlowId,
            ReconcileCashFlowRequest? request,
            ITresorerieService tresorerieService,
            ILogger<ReconcileCashFlow> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.ReconcileCashFlowAsync(
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
                "Flux de tresorerie reconcilie avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("ReconcileCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<ReconcileCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Marquer un flux comme reconcilie")
        .WithDescription("Marque un flux de tresorerie comme reconcilie avec le releve bancaire via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
