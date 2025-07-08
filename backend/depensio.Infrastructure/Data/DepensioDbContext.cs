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
    //public DbSet<Subscription> Subscription => Set<Subscription>();
    //public DbSet<Plan> Plan => Set<Plan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
