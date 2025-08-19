using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync(Guid boutiqueId);
}
