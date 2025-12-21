using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class UpdateRecurringCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/tresorerie/{boutiqueId}/recurring-cash-flows/{id:guid}", async (
            Guid boutiqueId,
            Guid id,
            UpdateRecurringCashFlowRequest request,
            ITresorerieService tresorerieService,
            ILogger<UpdateRecurringCashFlow> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.UpdateRecurringCashFlowAsync(
                id,
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie recurrent mis a jour avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("UpdateRecurringCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<UpdateRecurringCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Modifier un flux de tresorerie recurrent")
        .WithDescription("Modifie un flux de tresorerie recurrent existant via le microservice Tresorerie.")
        .RequireAuthorization();
    }
}
