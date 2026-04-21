namespace Common.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> BeginTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
}
