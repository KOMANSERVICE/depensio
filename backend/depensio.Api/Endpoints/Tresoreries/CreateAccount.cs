using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;
using Refit;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/accounts", async (
            Guid boutiqueId,
            CreateAccountRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateAccount> logger) =>
        {
            
            var applicationId = "depensio";
            var result = await tresorerieService.CreateAccountAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Compte de tresorerie cree avec succes",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/accounts/{result.Data!.Account.Id}", baseResponse);
            
        })
        //.AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("CreateTresorerieAccount")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateAccountResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Creer un compte de tresorerie")
        .WithDescription("Cree un nouveau compte de tresorerie pour une boutique via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
