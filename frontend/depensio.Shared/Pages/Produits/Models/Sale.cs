namespace depensio.Shared.Pages.Produits.Models;


public record GetSaleByBoutiqueResponse(IEnumerable<Sale> Sales);
public record Sale
(
    Guid Id,
    Guid BoutiqueId,
    string Title,
    string Description,
    decimal TotalPrice,
    IEnumerable<SaleItem> Items);

public record SaleItem
(
    Guid Id,
    Guid ProductId,
    decimal Price,
    int Quantity
);
