using Entities.DTO.Request.Day;

namespace Business;
public class DayBusiness : IBusiness<DayDTO, DayEntity>
{
    private readonly IMapper _mapper;

    public DayBusiness(IMapper mapper)
    {
        _mapper = mapper;
    }

    public DayEntity Add(DayDTO entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public DayEntity Update(DayDTO entity)
    {
        throw new NotImplementedException();
    }
}
