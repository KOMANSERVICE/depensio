using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows", async (
            Guid boutiqueId,
            CreateCashFlowRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.CreateCashFlowAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie cree avec succes",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/cash-flows/{result.Data!.CashFlow.Id}", baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("CreateCashFlow")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateCashFlowResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Creer un flux de tresorerie manuellement (depense)")
        .WithDescription("Cree un nouveau flux de tresorerie pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
