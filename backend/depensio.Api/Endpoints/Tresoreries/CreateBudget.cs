using depensio.Application.ApiExterne.Tresoreries;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateBudget : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/budgets", async (
            Guid boutiqueId,
            CreateBudgetRequest request,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.CreateBudgetAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Budget cree avec succes",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/budgets/{result.Data!.Budget.Id}", baseResponse);
        })
        .WithName("CreateBudget")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateBudgetResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Creer un budget")
        .WithDescription("Cree un nouveau budget pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
