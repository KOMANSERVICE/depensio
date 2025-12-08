using depensio.Application.Data;
using depensio.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace depensio.Infrastructure.Data;

public class DepensioDbContext : IdentityDbContext<ApplicationUser>, IDepensioDbContext
{

    public DepensioDbContext(DbContextOptions<DepensioDbContext> options)
        : base(options)
    {
    }

    public DbSet<UsersBoutique> UsersBoutiques => Set<UsersBoutique>();
    public DbSet<Boutique> Boutiques => Set<Boutique>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<BoutiqueSetting> BoutiqueSettings => Set<BoutiqueSetting>();
    public DbSet<ProductItem> ProductItems => Set<ProductItem>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<PlanMenu> PlanMenus => Set<PlanMenu>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ProfileMenu> ProfileMenus => Set<ProfileMenu>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
