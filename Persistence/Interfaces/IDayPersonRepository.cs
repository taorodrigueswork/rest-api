using Entities.Entity;
using Persistence.Interfaces.GenericRepository;

namespace Persistence.Interfaces;

public interface IDayPersonRepository : IGenericRepository<DayPersonEntity>
{
    Task DeleteByDayIdAsync(int dayId);
}
