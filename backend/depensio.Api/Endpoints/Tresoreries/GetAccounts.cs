using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

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
            
            var applicationId = "depensio";
            var result = await tresorerieService.GetAccountsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.IncludeInactive,
                queryParams.Type);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des comptes de tresorerie recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
            
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
