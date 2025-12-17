using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class UpdateAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/tresorerie/{boutiqueId}/accounts/{accountId}", async (
            Guid boutiqueId,
            Guid accountId,
            UpdateAccountRequest request,
            ITresorerieService tresorerieService,
            ILogger<UpdateAccount> logger) =>
        {
            try
            {
                var applicationId = "depensio";
                var result = await tresorerieService.UpdateAccountAsync(
                    accountId,
                    applicationId,
                    boutiqueId.ToString(),
                    request);

                if (!result.Success)
                {
                    return Results.BadRequest(result);
                }

                var baseResponse = ResponseFactory.Success(
                    result.Data,
                    "Compte de tresorerie modifie avec succes",
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
        .WithName("UpdateTresorerieAccount")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<UpdateAccountResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Modifier un compte de tresorerie")
        .WithDescription("Modifie un compte de tresorerie existant via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
