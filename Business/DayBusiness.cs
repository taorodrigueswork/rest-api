using Entities.DTO.Request.Day;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class DayBusiness : IBusiness<DayDto, DayEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<DayBusiness> _logger;
    private readonly IDayRepository _dayRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IDayPersonRepository _dayPersonRepository;
    public DayBusiness(IMapper mapper,
        ILogger<DayBusiness> logger,
        IDayRepository dayRepository,
        IPersonRepository personRepository,
        IScheduleRepository scheduleRepository,
        IDayPersonRepository dayPersonRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _dayRepository = dayRepository;
        _personRepository = personRepository;
        _scheduleRepository = scheduleRepository;
        _dayPersonRepository = dayPersonRepository;
    }

    public async Task<DayEntity> Add(DayDto entity)
    {
        var day = await _dayRepository.InsertAsync(_mapper.Map<DayEntity>(entity));

        _logger.LogInformation($"Added day ", entity);

        return day;
    }

    public async Task Delete(int id)
    {
        var day = await _dayRepository.FindByIdAsync(id);

        ArgumentNullException.ThrowIfNull(day, $"The day with id {id} was not found.");

        _logger.LogInformation($"Deleted day.", day);
        await _dayRepository.DeleteAsync(day);
    }

    public async Task<DayEntity> GetById(int id)
    {
        return await _dayRepository.FindByIdAsync(id);
    }

    public async Task<DayEntity?> Update(int id, DayDto entity)
    {
        var day = await _dayRepository.GetDayWithSubclassesAsync(id);

        ArgumentNullException.ThrowIfNull(day, $"The day with id {id} was not found.");

        // Delete all people from the day many-to-many relationship using a single query
        await _dayPersonRepository.DeleteByDayIdAsync(id);

        // Fetch only the required people from the database using their IDs
        var people = await _personRepository.GetPeopleAsync(entity.People);

        day.Day = entity.Day;
        day.Schedule = await _scheduleRepository.FindByIdAsync(entity.ScheduleId);

        // Assign the fetched people to the day
        day.People.Clear();// Clear the old list of people in the memory
        day.People.AddRange(people);

        await _dayRepository.UpdateAsync(day);

        _logger.LogInformation($"Updated day with ID {day.Id}", day);

        return day;
    }
}
