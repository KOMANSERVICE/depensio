using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetAccounts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/accounts", async (
            Guid boutiqueId,
            [AsParameters] GetAccountsQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetAccounts> logger) =>
        {
            try
            {
                var applicationId = "depensio";
                var result = await tresorerieService.GetAccountsAsync(
                    applicationId,
                    boutiqueId.ToString(),
                    queryParams.IncludeInactive,
                    queryParams.Type);

                if (!result.Success)
                {
                    return Results.BadRequest(result);
                }

                var baseResponse = ResponseFactory.Success(
                    result.Data,
                    "Liste des comptes de tresorerie recuperee avec succes",
                    StatusCodes.Status200OK);

                return Results.Ok(baseResponse);
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, "Erreur lors de l'appel au microservice Tresorerie: {StatusCode} - {Content}", ex.StatusCode, ex.Content);
                return Results.Problem(
                    detail: ex.Content ?? "Erreur interne du service Tresorerie",
                    statusCode: (int)ex.StatusCode,
                    title: "Erreur du microservice Tresorerie");
            }
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetTresorerieAccounts")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetAccountsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les comptes de tresorerie")
        .WithDescription("Recupere la liste des comptes de tresorerie pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetAccountsQueryParams(
    bool IncludeInactive = false,
    AccountType? Type = null
);
