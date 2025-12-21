using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class ApproveCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/approve", async (
            Guid boutiqueId,
            Guid cashFlowId,
            ITresorerieService tresorerieService,
            ILogger<ApproveCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.ApproveCashFlowAsync(
                cashFlowId,
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie approuve avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("ApproveCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<ApproveCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Approuver un flux de tresorerie")
        .WithDescription("Approuve un flux de tresorerie en attente de validation via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
