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
    //public DbSet<Subscription> Subscription => Set<Subscription>();
    //public DbSet<Plan> Plan => Set<Plan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
