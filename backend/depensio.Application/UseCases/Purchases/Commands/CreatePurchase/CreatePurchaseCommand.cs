using depensio.Application.UseCases.Purchases.DTOs;

namespace depensio.Application.UseCases.Purchases.Commands.CreatePurchase;

public record CreatePurchaseCommand(PurchaseDTO Purchase)
    : ICommand<CreatePurchaseResult>;

public record CreatePurchaseResult(Guid Id);
 
public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.Purchase)
            .NotNull().NotEmpty().WithMessage("L'achat est obligatoire.");

        RuleFor(x => x.Purchase.Title)
            .NotNull().NotEmpty().WithMessage("Le titre est obligatoire.");

        RuleFor(x => x.Purchase.DateAchat)
            .NotNull().WithMessage("La date d'achat est obligatoire.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("La date d'achat ne peut pas être postérieure à la date du jour.");

        RuleFor(x => x.Purchase.SupplierName)
            .NotNull().NotEmpty().WithMessage("Le nom du fournisseur est obligatoire.");

        RuleFor(x => x.Purchase.Items)
            .NotNull().WithMessage("La liste des produits est obligatoire.")
            .NotEmpty().WithMessage("Vous devez sélectionner au moins un produit pour enregistrer un achat.")
            .Must(items => items != null && items.All(item => item.ProductId != Guid.Empty))
            .WithMessage("Tous les articles doivent avoir un produit sélectionné.");

        RuleFor(x => x.Purchase.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
