using depensio.Application.UseCases.Purchases.DTOs;

namespace depensio.Application.UseCases.Purchases.Commands.CreatePurchase;

public record CreatePurchaseCommand(PurchaseDTO Purchase)
    : ICommand<CreatePurchaseResult>;

public record CreatePurchaseResult(Guid Id);
 
public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.Purchase).NotNull().WithMessage("Purchase is required");
        RuleFor(x => x.Purchase.Title).NotNull().WithMessage("Title is required");
        RuleFor(x => x.Purchase.DateAchat)
        .NotNull().WithMessage("La date d'achat est requise")
        .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
        .WithMessage("La date d'achat ne peut pas être postérieure à la date du jour.");
        RuleFor(x => x.Purchase.SupplierName).NotNull().WithMessage("SupplierName is required");
        RuleFor(x => x.Purchase.Items).NotEmpty().WithMessage("Items is required");
        RuleFor(x => x.Purchase.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");
    }
}
