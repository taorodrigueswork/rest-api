using Entities.Entity;

namespace Persistence.Interfaces;

public interface IDayRepository : IGenericRepository<DayEntity>
{
    Task<DayEntity> GetDayWithSubclassesAsync(int id);
}
