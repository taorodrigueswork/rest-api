﻿using System.Linq.Expressions;

namespace Persistence.Interfaces;

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Insert data in database
    /// </summary>
    /// <param name="entity">Object that will inserted</param>
    /// <returns>Inserted object</returns>
    Task<T> InsertAsync(T entity);

    /// <summary>
    /// Find data from database by ProductEntityId
    /// </summary>
    /// <param name="primaryKey">ProductEntityId to find</param>
    /// <returns>Located object/returns>
    Task<T> FindByIdAsync(object primaryKey);

    /// <summary>
    /// Find data from database by Filter
    /// </summary>
    /// <param name="whereClause">Filter to find data</param>
    /// <returns>Located object</returns>
    Task<T> FindAsync(Expression<Func<T, bool>> whereClause);

    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> whereClause);

    /// <summary>
    /// Delete data from database
    /// </summary>
    /// <param name="entity">Object to delete</param>
    /// <param name="predicate">Filter to delete in specific case</param>
    /// <returns></returns>
    Task DeleteAsync(T entity, Func<T, bool> predicate = default!);

    /// <summary>
    /// Update data from database
    /// </summary>
    /// <param name="entity">Object to update</param>
    /// <param name="predicate">Filter to update in specific case</param>
    /// <returns></returns>
    Task UpdateAsync(T entity, Func<T, bool> predicate = default!);

    Task UpdateRangeAsync(T entity, Func<T, bool> predicate = default!);
}