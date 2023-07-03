using Entities.Entity;
using Persistence.Interfaces.GenericRepository;

namespace Persistence.Interfaces;

public interface IDayRepository : IGenericRepository<DayEntity>
{
    Task<DayEntity> GetDayWithSubclassesAsync(int id);
}
