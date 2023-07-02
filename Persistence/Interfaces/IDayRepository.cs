using Entities.Entity;

namespace Persistence.Interfaces;

public interface IDayRepository : IGenericRepository<DayEntity>
{
    Task<DayEntity> GetDayWithSubclasses(int id);

    Task<List<PersonEntity>> GetPeopleAsync(List<int> ids);
}
