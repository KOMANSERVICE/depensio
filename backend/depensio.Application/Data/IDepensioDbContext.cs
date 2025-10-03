namespace depensio.Application.Data;

public interface IDepensioDbContext
{
    DbSet<UsersBoutique> UsersBoutiques { get; }
    DbSet<Boutique> Boutiques { get; }
    DbSet<Product> Products { get; }
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<BoutiqueSetting> BoutiqueSettings { get; }
    DbSet<ProductItem> ProductItems { get; }
    DbSet<Plan> Plans { get; }
    DbSet<Feature> Features { get; }
    DbSet<PlanFeature> PlanFeatures { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Menu> Menus { get; }
    DbSet<PlanMenu> PlanMenus { get; }
    DbSet<Profile> Profiles { get; }
    DbSet<ProfileMenu> ProfileMenus { get; }
}
