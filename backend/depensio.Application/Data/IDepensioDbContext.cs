namespace depensio.Application.Data;

public interface IDepensioDbContext
{
    DbSet<UsersBoutique> UsersBoutiques { get; }
    DbSet<Boutique> Boutiques { get; }
    DbSet<Product> Products { get; }
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
}
