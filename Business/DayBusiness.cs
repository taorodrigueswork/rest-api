using Entities.DTO.Request.Day;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class DayBusiness : IBusiness<DayDTO, DayEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<DayBusiness> _logger;
    private readonly IGenericRepository<DayEntity> _repository;
    public DayBusiness(IMapper mapper,
        ILogger<DayBusiness> logger,
        IGenericRepository<DayEntity> repository)
    {
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
    }

    public async Task<DayEntity> Add(DayDTO entity)
    {
        var day = await _repository.InsertAsync(_mapper.Map<DayEntity>(entity));

        _logger.LogInformation($"Added day ", entity);

        return day;
    }

    public async Task<DayEntity> Delete(int id)
    {
        var day = await _repository.FindByIdAsync(id);

        if (day != null)
        {
            _logger.LogInformation($"Deleted day.", day);
            await _repository.DeleteAsync(day);
        }

        _logger.LogWarning($"The day with id {id} was not found.");
        return day;
    }

    public async Task<DayEntity> GetById(int id)
    {
        return await _repository.FindByIdAsync(id);
    }

    public Task<DayEntity> Update(int id, DayDTO entity)
    {
        throw new NotImplementedException();
    }
}
