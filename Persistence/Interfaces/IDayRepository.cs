using Entities.Entity;
using Persistence.Interfaces.GenericRepository;

namespace Persistence.Interfaces;

public interface IDayRepository : IGenericRepository<DayEntity>
{
    Task<List<DayEntity>> GetDaysAsync(List<int> days);
    Task<DayEntity> GetDayWithSubclassesAsync(int id);
}
