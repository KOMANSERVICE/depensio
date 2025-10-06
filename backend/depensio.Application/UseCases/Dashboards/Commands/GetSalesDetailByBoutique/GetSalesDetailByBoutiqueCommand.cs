using depensio.Application.UseCases.Dashboard.DTOs;

namespace depensio.Application.UseCases.Dashboards.Commands.GetSalesDetailByBoutique;

public record GetSalesDetailByBoutiqueCommand(Guid BoutiqueId, SaleRequestDTO SaleRequest)
    : ICommand<GetSalesDetailByBoutiqueResult>;

public record GetSalesDetailByBoutiqueResult(IEnumerable<SaleDashboardDTO> SalesDetails);

public class GetSalesDetailByBoutiqueValidator : AbstractValidator<GetSalesDetailByBoutiqueCommand>
{
    public GetSalesDetailByBoutiqueValidator()
    {
        RuleFor(x => x.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required.");
        RuleFor(x => x.SaleRequest).NotNull().WithMessage("SaleRequest is required.");
        RuleFor(x => x.SaleRequest.StartDate).NotEmpty().WithMessage("StartDate is required.");
        RuleFor(x => x.SaleRequest.EndDate).NotEmpty().WithMessage("EndDate is required.");
        RuleFor(x => x.SaleRequest.StartDate).LessThanOrEqualTo(x => x.SaleRequest.EndDate)
            .WithMessage("StartDate must be less than or equal to EndDate.");
    }
}
