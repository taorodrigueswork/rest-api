using Microsoft.EntityFrameworkCore;
using Persistence.Interfaces;
using System.Linq.Expressions;

namespace Persistence.Repository.GenericRepository;


public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, new()
{
    public readonly DbContext Context;

    protected GenericRepository(DbContext dbContext)
    {
        Context = dbContext;
    }

    public async Task<T> InsertAsync(T entity)
    {
        Context.Attach(entity);
        var entityEntry = await Context.Set<T>().AddAsync(entity);

        await SaveDatabaseAsync();

        return entityEntry.Entity;
    }

    public async Task<T> FindByIdAsync(object primaryKey)
    {
        var entity = await Context.Set<T>().FindAsync(primaryKey);

        if (entity != null)
        {
            return entity;
        }

        return new T();
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>> whereClause)
    {
        var entity = await Context.Set<T>().AsNoTracking().FirstOrDefaultAsync(whereClause);

        if (entity != null)
        {
            return entity;
        }

        return new T();
    }

    public async Task DeleteAsync(T entity, Func<T, bool> predicate = default!)
    {
        if (predicate != default)
        {
            DetachLocal(predicate);
        }

        Context.Set<T>().Remove(entity);

        await SaveDatabaseAsync();
    }

    public async Task UpdateAsync(T entity, Func<T, bool> predicate = default!)
    {
        if (predicate != default)
        {
            DetachLocal(predicate);
        }

        Context.Set<T>().Update(entity);

        await SaveDatabaseAsync();
    }

    /// <summary>
    /// Save changes on database
    /// </summary>
    /// <returns></returns>
    private async Task SaveDatabaseAsync()
    {
        await Context.SaveChangesAsync();
    }


    /// <summary>
    /// Detach local data from database
    /// </summary>
    /// <param name="predicate">Filter to find data to detach</param>
    private void DetachLocal(Func<T, bool> predicate)
    {
        var local = Context.Set<T>().Local.FirstOrDefault(predicate);

        if (local != null)
        {
            Context.Entry(local).State = EntityState.Detached;
        }
    }
}

