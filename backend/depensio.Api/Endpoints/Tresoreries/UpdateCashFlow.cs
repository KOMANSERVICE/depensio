using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class UpdateCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}", async (
            Guid boutiqueId,
            Guid cashFlowId,
            UpdateCashFlowRequest request,
            ITresorerieService tresorerieService,
            ILogger<UpdateCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.UpdateCashFlowAsync(
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
                "Flux de tresorerie modifie avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("UpdateCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<UpdateCashFlowResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Modifier un flux de tresorerie en brouillon")
        .WithDescription("Modifie un flux de tresorerie existant pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
