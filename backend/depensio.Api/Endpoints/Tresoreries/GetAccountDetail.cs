using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetAccountDetail : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/accounts/{accountId}", async (
            Guid boutiqueId,
            Guid accountId,
            [AsParameters] GetAccountDetailQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetAccountDetail> logger) =>
        {
            
            var applicationId = "depensio";
            var result = await tresorerieService.GetAccountDetailAsync(
                accountId,
                applicationId,
                boutiqueId.ToString(),
                queryParams.FromDate,
                queryParams.ToDate);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Detail du compte de tresorerie recupere avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
            
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetTresorerieAccountDetail")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetAccountDetailResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Consulter le detail d'un compte de tresorerie")
        .WithDescription("Recupere le detail d'un compte de tresorerie avec ses mouvements recents et l'evolution du solde")
        .RequireAuthorization();
    }
}

public record GetAccountDetailQueryParams(
    DateTime? FromDate = null,
    DateTime? ToDate = null
);
