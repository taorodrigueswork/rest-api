
using Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interfaces;
using Persistence.Repository.GenericRepository;

namespace Persistence.Repository;

public class ScheduleRepository : GenericRepository<ScheduleEntity>, IScheduleRepository
{

    public ScheduleRepository(ApiContext apiContext)
        : base(apiContext)
    {

    }

    public async Task<ScheduleEntity> GetByIdWithSubclassesAsync(int id)
    {
        return (await Context.Set<ScheduleEntity>()
                             .Include(p => p.Days)
                                  .ThenInclude(p => p.People)
                             .Where(p => p.Id == id)
                             .FirstOrDefaultAsync()!)!;
    }
}
