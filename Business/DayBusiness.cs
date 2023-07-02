using Entities.DTO.Request.Day;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class DayBusiness : IBusiness<DayDTO, DayEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<DayBusiness> _logger;
    private readonly IDayRepository _dayRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IGenericRepository<DayPersonEntity> _dayPersonRepository;
    public DayBusiness(IMapper mapper,
        ILogger<DayBusiness> logger,
        IDayRepository dayRepository,
        IScheduleRepository scheduleRepository,
        IGenericRepository<DayPersonEntity> dayPersonRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _dayRepository = dayRepository;
        _scheduleRepository = scheduleRepository;
        _dayPersonRepository = dayPersonRepository;
    }

    public async Task<DayEntity> Add(DayDTO entity)
    {
        var day = await _dayRepository.InsertAsync(_mapper.Map<DayEntity>(entity));

        _logger.LogInformation($"Added day ", entity);

        return day;
    }

    public async Task<DayEntity> Delete(int id)
    {
        var day = await _dayRepository.FindByIdAsync(id);

        if (day != null)
        {
            _logger.LogInformation($"Deleted day.", day);
            await _dayRepository.DeleteAsync(day);
        }

        _logger.LogWarning($"The day with id {id} was not found.");
        return day;
    }

    public async Task<DayEntity> GetById(int id)
    {
        return await _dayRepository.FindByIdAsync(id);
    }

    public async Task<DayEntity> Update(int id, DayDTO entity)
    {
        var day = await _dayRepository.GetDayWithSubclasses(id);

        if (day == null)
        {
            _logger.LogWarning($"The day with id {id} was not found.");
            return null;
        }

        day.Day = entity.Day;
        day.Schedule = await _scheduleRepository.FindByIdAsync(entity.ScheduleId);

        // Delete all people from the day many to many relationship
        foreach (var person in day.People)
        {
            await _dayPersonRepository.DeleteAsync(new DayPersonEntity
            {
                DayId = id,
                PersonId = person.Id
            });
        }

        // Clear the old list of people in the memory
        day.People.Clear();

        // Recover the new list of people from the database
        day.People = await _dayRepository.GetPeopleAsync(entity.People);

        await _dayRepository.UpdateAsync(day);

        _logger.LogInformation($"Updated day with ID {day.Id}", day);

        return day;
    }
}
