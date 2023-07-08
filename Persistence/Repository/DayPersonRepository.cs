
using Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interfaces;
using Persistence.Repository.GenericRepository;

namespace Persistence.Repository;

public class DayPersonRepository : GenericRepository<DayPersonEntity>, IDayPersonRepository
{
    public DayPersonRepository(ApiContext apiContext)
        : base(apiContext)
    {

    }

    public async Task DeleteByDayIdAsync(int dayId)
    {
        var dayPersons = await Context.Set<DayPersonEntity>()
            .Where(dp => dp.DayId == dayId)
        .ToListAsync();

        Context.Set<DayPersonEntity>().RemoveRange(dayPersons);

        await Context.SaveChangesAsync();
    }
}
