using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.Interfaces;

public interface IProductService
{
    Task<Product?> GetOneProductAsync(Guid boutiqueId, Guid productId);
    Task<IEnumerable<Product>> GetProductsAsync(Guid boutiqueId);
}
