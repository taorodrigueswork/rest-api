namespace Business.Interfaces;

public interface IBusiness<T, U> where T : class where U : class
{
    U Add(T entity);
    U Update(T entity);
    void Delete(int id);
}