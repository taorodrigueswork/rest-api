namespace Business.Interfaces;

public interface IBusiness<T, U> where T : class where U : class
{
    Task<U> Add(T entity);
    Task<U> Update(int id, T entity);
    Task<U> Delete(int id);
    Task<U> GetById(int id);
}