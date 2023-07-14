
using Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interfaces;
using Persistence.Repository.GenericRepository;

namespace Persistence.Repository;

public class DayRepository : GenericRepository<DayEntity>, IDayRepository
{
    public DayRepository(ApiContext apiContext)
        : base(apiContext)
    {

    }

    public async Task<List<DayEntity>> GetDaysAsync(List<int> days)
    {
        return (await Context.Set<DayEntity>()
                             .Where(p => days.Any(id => id == p.Id))
                             .ToListAsync());
    }

    public async Task<DayEntity> GetDayWithSubclassesAsync(int id)
    {
        return (await Context.Set<DayEntity>()
           .Include(p => p.People)
           .Include(p => p.Schedule)
           .Where(p => p.Id == id).FirstOrDefaultAsync()!)!;
    }
}
