using depensio.Application.Interfaces;
using depensio.Infrastructure.Data;

namespace depensio.Infrastructure.Repositories;

public class UnitOfWork(DepensioDbContext _context) : IUnitOfWork
{
    public async Task SaveChangesDataAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
