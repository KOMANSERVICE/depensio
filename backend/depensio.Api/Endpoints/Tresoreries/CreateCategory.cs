using depensio.Application.ApiExterne.Tresoreries;
using IDR.Library.BuildingBlocks.Exceptions;
using IDR.Library.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateCategory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/categories", async (
            [FromHeader(Name = "X-Boutique-Id")] string boutiqueId,
            CreateCategoryRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateCategory> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.CreateCategoryAsync(
                applicationId,
                boutiqueId,
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Categorie creee avec succes",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/categories/{result.Data!.Category.Id}", baseResponse);
        })
        .WithName("CreateTresorerieCategory")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateCategoryResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Creer une categorie de tresorerie")
        .WithDescription("Cree une nouvelle categorie pour les operations de tresorerie via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
