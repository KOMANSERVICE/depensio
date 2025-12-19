using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Api.Endpoints.Tresoreries;

public class CreateTransfer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/tresorerie/{boutiqueId}/cash-flows/transfer", async (
            Guid boutiqueId,
            CreateTransferRequest request,
            ITresorerieService tresorerieService,
            ILogger<CreateTransfer> logger) =>
        {
            var applicationId = "depensio";
            var result = await tresorerieService.CreateTransferAsync(
                applicationId,
                boutiqueId.ToString(),
                request);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            var baseResponse = ResponseFactory.Success(
                result.Data,
                "Transfert cree avec succes",
                StatusCodes.Status201Created);

            return Results.Created($"/tresorerie/{boutiqueId}/cash-flows/transfer/{result.Data!.Transfer.Id}", baseResponse);
        })
        .WithName("CreateTransfer")
        .WithTags("Tresorerie")
        .Produces<BaseResponse<CreateTransferResponse>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Creer un transfert entre comptes")
        .WithDescription("Cree un nouveau transfert entre deux comptes de tresorerie via le microservice Tresorerie")
        .RequireAuthorization();
    }
}
