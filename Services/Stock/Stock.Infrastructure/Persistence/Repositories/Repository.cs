using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Stock.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    protected readonly StockDbContext Context;

    public Repository(StockDbContext context)
    {
        Context = context;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Update(entity);
        await Task.CompletedTask;
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        Context.Set<T>().Remove(entity);
        return true;
    }
}
