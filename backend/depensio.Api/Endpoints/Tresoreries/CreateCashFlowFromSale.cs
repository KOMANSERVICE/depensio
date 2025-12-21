using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateCashFlowFromSale : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/from-sale", async (
            Guid boutiqueId,
            CreateCashFlowFromSaleRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateCashFlowFromSale> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.CreateCashFlowFromSaleAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie cree automatiquement depuis la vente",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/cash-flows/{result.Data!.CashFlow.Id}", baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("CreateCashFlowFromSale")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateCashFlowFromSaleResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithSummary("Creer un flux de tresorerie automatiquement depuis une vente")
        .WithDescription("Cree un nouveau flux de tresorerie (revenu) automatiquement a partir d'une vente validee via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
