using Entities.Entity;
using Persistence.Interfaces.GenericRepository;

namespace Persistence.Interfaces;

public interface IScheduleRepository : IGenericRepository<ScheduleEntity>
{
    Task<ScheduleEntity> GetByIdWithSubclassesAsync(int id);
}
