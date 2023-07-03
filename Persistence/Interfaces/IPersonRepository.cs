using Entities.Entity;

namespace Persistence.Interfaces;

public interface IPersonRepository : IGenericRepository<PersonEntity>
{
    Task<List<PersonEntity>> GetPeopleAsync(List<int> ids);
}
