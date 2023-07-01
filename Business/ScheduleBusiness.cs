using Entities.DTO.Request.Schedule;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class ScheduleBusiness : IBusiness<ScheduleDTO, ScheduleEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleBusiness> _logger;
    private readonly IGenericRepository<ScheduleEntity> _repository;

    public ScheduleBusiness(IMapper mapper,
        ILogger<ScheduleBusiness> logger,
        IGenericRepository<ScheduleEntity> repository)
    {
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
    }

    public ScheduleEntity Add(ScheduleDTO entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public ScheduleEntity Update(ScheduleDTO entity)
    {
        throw new NotImplementedException();
    }
}
