
using Entities.Entity;
using Persistence.Context;
using Persistence.Interfaces;
using Persistence.Repository.GenericRepository;

namespace Persistence.Repository
{
    public class ScheduleRepository : GenericRepository<ScheduleEntity>, IScheduleRepository
    {

        public ScheduleRepository(ApiContext apiContext)
            : base(apiContext)
        {

        }
    }
}
