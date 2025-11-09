using depensio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Infrastructure;

public class DepensioDbContextFactory : IDesignTimeDbContextFactory<DepensioDbContext>
{
    public DepensioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DepensioDbContext>();

        // Chaine de connexion "factice" ou minimale pour EF Core
        optionsBuilder.UseNpgsql("");

        return new DepensioDbContext(optionsBuilder.Options);
    }
}
