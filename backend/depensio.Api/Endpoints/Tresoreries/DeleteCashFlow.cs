using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class DeleteCashFlow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}", async (
            Guid boutiqueId,
            Guid cashFlowId,
            ITresorerieService tresorerieService,
            ILogger<DeleteCashFlow> logger) =>
        {

            var applicationId = "depensio";
            var result = await tresorerieService.DeleteCashFlowAsync(
                cashFlowId,
                applicationId,
                boutiqueId.ToString());

            if (!result.IsSuccessStatusCode)
            {
                throw new BadRequestException("Erreur lors de la suppression du flux de tresorerie");
            }

            return Results.NoContent();

        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("DeleteCashFlow")
        .WithTags("Tresorerie")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Supprimer un flux de tresorerie en brouillon")
        .WithDescription("Supprime un flux de tresorerie en brouillon pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
