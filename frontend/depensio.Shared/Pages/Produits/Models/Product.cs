using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

public record ProductBarcodeDTO(Guid ProductId, string Name, List<ProductItemBarcodeDTO> ProductItems);

public record GetProductItemByBoutiqueResponse(IEnumerable<ProductBarcodeDTO> ProductBarcodes);

public record ProductItemBarcodeDTO(Guid Id, string Barcode);

