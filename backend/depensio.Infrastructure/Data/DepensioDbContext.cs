using depensio.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace depensio.Infrastructure.Data;

public class DepensioDbContext : IdentityDbContext<ApplicationUser>
{

    public DepensioDbContext(DbContextOptions<DepensioDbContext> options)
        : base(options)
    {
    }

    public DbSet<UsersBoutique> UsersBoutiques => Set<UsersBoutique>();
    public DbSet<Boutique> Boutiques => Set<Boutique>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
