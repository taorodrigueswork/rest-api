using Entities.DTO.Request.Schedule;

namespace Business;
public class ScheduleBusiness : IBusiness<ScheduleDTO, ScheduleEntity>
{
    private readonly IMapper _mapper;

    public ScheduleBusiness(IMapper mapper)
    {
        _mapper = mapper;
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
