namespace depensio.Shared.Pages.Produits.Models;

public record GetProductByUserResponse(IEnumerable<Product> Products);
public record CreateProductRequest(Product Product);
public record CreateOrUpdateProductResponse(Guid Id);
public record Product
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int Stock { get; set; } = 0;
    public List<string> Barcodes { get; set; } = new();
}

public record ProductBarcode(Guid ProductId, string Name, List<ProductItemBarcode> ProductItems);

public record GetProductItemByBoutiqueResponse(IEnumerable<ProductBarcode> ProductBarcodes);

public record ProductItemBarcode(Guid Id, string Barcode);
public record ProductItem
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public Guid ProductId { get; set; } = Guid.Empty;
    public int BarcodeCount { get; set; }
    public decimal DiscountValue { get; set; }
    public DiscountType DiscountType { get; set; }
}


public record Barcode(Guid ProductId, List<string> Barcodes);
public record CreateCodeBarreRequest(ProductItem ProductItem);
public record CreateCodeBarreResponse(Barcode Barcode);

