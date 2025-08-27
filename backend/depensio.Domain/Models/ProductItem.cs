using depensio.Domain.Enums;

namespace depensio.Domain.Models;

public class ProductItem : Entity<ProductItemId>
{
    public ProductId ProductId { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Available;
    public string Barcode { get; set; } = string.Empty;
    public Product Product { get; set; }
}
