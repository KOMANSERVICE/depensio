using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetTreasuryDashboard : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/dashboard", async (
            Guid boutiqueId,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetTreasuryDashboardAsync(
                applicationId,
                boutiqueId.ToString());

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Tableau de bord tresorerie recupere avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("GetTreasuryDashboard")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<TreasuryDashboardDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Tableau de bord tresorerie")
        .WithDescription("Recupere le tableau de bord tresorerie avec les totaux, flux, alertes et evolution")
        .RequireAuthorization();
    }
}
