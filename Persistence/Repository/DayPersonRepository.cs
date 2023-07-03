
using Entities.Entity;
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
}
