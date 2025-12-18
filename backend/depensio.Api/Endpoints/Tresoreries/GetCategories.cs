using depensio.Application.ApiExterne.Tresoreries;
using IDR.Library.BuildingBlocks.Exceptions;
using IDR.Library.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class GetCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/categories", async (
            [FromHeader(Name = "X-Boutique-Id")] string boutiqueId,
            [AsParameters] GetCategoriesQueryParams queryParams,
            ITresorerieService tresorerieService,
            ILogger<GetCategories> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.GetCategoriesAsync(
                applicationId,
                boutiqueId,
                queryParams.Type,
                queryParams.IncludeInactive);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Liste des categories recuperee avec succes",
                StatusCodes.Status200OK);

            return Results.Ok(baseResponse);
        })
        .WithName("GetTresorerieCategories")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<GetCategoriesResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Lister les categories de tresorerie")
        .WithDescription("Recupere la liste des categories de tresorerie via le microservice Tresorerie")
        .RequireAuthorization();
    }
}

public record GetCategoriesQueryParams(
    CategoryType? Type = null,
    bool IncludeInactive = false
);
