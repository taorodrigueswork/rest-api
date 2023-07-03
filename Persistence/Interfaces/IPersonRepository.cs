using Entities.Entity;
using Persistence.Interfaces.GenericRepository;

namespace Persistence.Interfaces;

public interface IPersonRepository : IGenericRepository<PersonEntity>
{
    Task<List<PersonEntity>> GetPeopleAsync(List<int> ids);
}
