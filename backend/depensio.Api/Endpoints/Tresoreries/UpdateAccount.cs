using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
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
            
            var applicationId = "depensio";
            var result = await tresorerieService.UpdateAccountAsync(
                accountId,
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Compte de tresorerie modifie avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
            
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
