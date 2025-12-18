using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetAccountBalance : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/accounts/{accountId}/balance", async (
            Guid boutiqueId,
            Guid accountId,
            ITresorerieService tresorerieService,
            ILogger<GetAccountBalance> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetAccountBalanceAsync(
                accountId,
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Solde du compte de tresorerie recupere avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
           
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetTresorerieAccountBalance")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<AccountBalanceDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Consulter le solde en temps reel d'un compte de tresorerie")
        .WithDescription("Recupere le solde actuel d'un compte avec les variations du jour et du mois, et l'etat des alertes")
        .RequireAuthorization();
    }
}
