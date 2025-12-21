using depensio.Application.ApiExterne.Tresoreries;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateRecurringCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/recurring-cash-flows", async (
            Guid boutiqueId,
            CreateRecurringCashFlowRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateRecurringCashFlow> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.CreateRecurringCashFlowAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie recurrent cree avec succes",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/recurring-cash-flows/{result.Data!.RecurringCashFlow.Id}", baseResponse);
        })
        .WithName("CreateRecurringCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateRecurringCashFlowResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Creer un flux de tresorerie recurrent")
        .WithDescription("Cree un nouveau flux de tresorerie recurrent pour une boutique via le microservice Tresorerie. Ce flux sera automatiquement genere selon la frequence definie.")
        .RequireAuthorization();
    }
}
