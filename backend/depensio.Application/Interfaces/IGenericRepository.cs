using System.Linq.Expressions;

namespace depensio.Application.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddDataAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeDataAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void UpdateData(TEntity entity, CancellationToken cancellationToken = default);
    void UpdateRangeData(IEnumerable<TEntity> entities);
    Task DeleteDataAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
