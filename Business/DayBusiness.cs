using Entities.DTO.Request.Day;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;
using Persistence.Interfaces.GenericRepository;

namespace Business;
public class DayBusiness : IBusiness<DayDto, DayEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<DayBusiness> _logger;
    private readonly IDayRepository _dayRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IGenericRepository<DayPersonEntity> _dayPersonRepository;
    public DayBusiness(IMapper mapper,
        ILogger<DayBusiness> logger,
        IDayRepository dayRepository,
        IPersonRepository personRepository,
        IScheduleRepository scheduleRepository,
        IGenericRepository<DayPersonEntity> dayPersonRepository)
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

    public async Task<DayEntity?> Delete(int id)
    {
        var day = await _dayRepository.FindByIdAsync(id);

        if (day != null)
        {
            _logger.LogInformation($"Deleted day.", day);
            await _dayRepository.DeleteAsync(day);
        }
        else
        {
            _logger.LogWarning($"The day with id {id} was not found.");
        }
        return day;
    }

    public async Task<DayEntity> GetById(int id)
    {
        return await _dayRepository.FindByIdAsync(id);
    }

    public async Task<DayEntity?> Update(int id, DayDto entity)
    {
        var day = await _dayRepository.GetDayWithSubclassesAsync(id);

        if (day == null)
        {
            _logger.LogWarning($"The day with id {id} was not found.");
            return day;
        }

        // Delete all people from the day many to many relationship
        foreach (var person in day.People)
        {
            await _dayPersonRepository.DeleteAsync(new DayPersonEntity
            {
                DayId = id,
                PersonId = person.Id
            });
        }

        day.Day = entity.Day;
        day.Schedule = await _scheduleRepository.FindByIdAsync(entity.ScheduleId);
        day.People.Clear();// Clear the old list of people in the memory
        day.People = await _personRepository.GetPeopleAsync(entity.People);// Recover the new list of people from the database

        await _dayRepository.UpdateAsync(day);

        _logger.LogInformation($"Updated day with ID {day.Id}", day);

        return day;
    }
}
