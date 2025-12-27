using depensio.Application.ApiExterne.Tresoreries;
using depensio.Infrastructure.Filters;
using IDR.Library.BuildingBlocks.Exceptions;

namespace depensio.Api.Endpoints.Tresoreries;

public class ExportCashFlows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tresorerie/{boutiqueId}/cash-flows/export", async (
            Guid boutiqueId,
            [AsParameters] ExportCashFlowsQueryParams queryParams,
            ITresorerieService tresorerieService) =>
        {
            var applicationId = "depensio";
            var response = await tresorerieService.ExportCashFlowsAsync(
                applicationId,
                boutiqueId.ToString(),
                queryParams.Format ?? "csv",
                queryParams.Columns,
                queryParams.Type,
                queryParams.Status,
                queryParams.AccountId,
                queryParams.CategoryId,
                queryParams.StartDate,
                queryParams.EndDate,
                queryParams.Search);

            if (!response.IsSuccessStatusCode)
            {
                throw new BadRequestException("Erreur lors de l'exportation des flux de tresorerie");
            }

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "text/csv";
            var contentDisposition = response.Content.Headers.ContentDisposition;
            var fileName = contentDisposition?.FileName?.Trim('"') ?? $"cash-flows-export.{queryParams.Format ?? "csv"}";

            var stream = await response.Content.ReadAsStreamAsync();
            return Results.File(stream, contentType, fileName);
        })
        .AddEndpointFilter<BoutiqueAuthorizationFilter>()
        .WithName("ExportCashFlows")
        .WithTags("Tresorerie")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Exporter les flux de tresorerie")
        .WithDescription("Exporte les flux de tresorerie au format CSV ou Excel")
        .RequireAuthorization();
    }
}

public record ExportCashFlowsQueryParams(
    string? Format = "csv",
    string? Columns = null,
    CashFlowTypeExtended? Type = null,
    CashFlowStatusExtended? Status = null,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? Search = null
);
