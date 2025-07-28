
using depensio.Application.Interfaces;
using depensio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace depensio.Infrastructure.Repositories;
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;
    public GenericRepository(DepensioDbContext context)
    {
        _dbSet = context.Set<TEntity>();
    }
    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).FirstOrDefaultAsync();
    }
    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbSet.ToListAsync();

    public async Task AddDataAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void UpdateData(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteDataAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task AddRangeDataAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void UpdateRangeData(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual Task<List<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
}
