namespace depensio.Application.Data;

public interface IDepensioDbContext
{
    DbSet<UsersBoutique> UsersBoutiques { get; }
    DbSet<Boutique> Boutiques { get; }
}
