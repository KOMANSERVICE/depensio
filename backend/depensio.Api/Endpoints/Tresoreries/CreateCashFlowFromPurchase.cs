using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateCashFlowFromPurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/from-purchase", async (
            Guid boutiqueId,
            CreateCashFlowFromPurchaseRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateCashFlowFromPurchase> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.CreateCashFlowFromPurchaseAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Flux de tresorerie cree automatiquement depuis l'achat",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/cash-flows/{result.Data!.CashFlow.Id}", baseResponse);

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("CreateCashFlowFromPurchase")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateCashFlowFromPurchaseResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithSummary("Creer un flux de tresorerie automatiquement depuis un achat")
        .WithDescription("Cree un nouveau flux de tresorerie (depense) automatiquement a partir d'un achat valide via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
