namespace depensio.Application.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesDataAsync(CancellationToken cancellationToken = default);
}
