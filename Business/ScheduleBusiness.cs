using Entities.DTO.Request.Schedule;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class ScheduleBusiness : IBusiness<ScheduleDto, ScheduleEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleBusiness> _logger;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IDayRepository _dayRepository;

    public ScheduleBusiness(IMapper mapper,
        ILogger<ScheduleBusiness> logger,
        IScheduleRepository repository,
        IDayRepository dayRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _scheduleRepository = repository;
        _dayRepository = dayRepository;
    }
    public async Task<ScheduleEntity> Add(ScheduleDto scheduleDto)
    {
        var schedule = await _scheduleRepository.InsertAsync(_mapper.Map<ScheduleEntity>(scheduleDto));

        _logger.LogInformation($"Added schedule ", schedule);

        return schedule;
    }

    public async Task Delete(int id)
    {
        var schedule = await _scheduleRepository.FindByIdAsync(id);

        ArgumentNullException.ThrowIfNull(schedule, $"The schedule with id {id} was not found.");

        await _scheduleRepository.DeleteAsync(schedule);

        _logger.LogInformation($"Deleted schedule.", schedule);
    }

    public async Task<ScheduleEntity?> GetById(int id) => await _scheduleRepository.GetByIdWithSubclassesAsync(id);

    public async Task<ScheduleEntity?> Update(int id, ScheduleDto scheduleDto)
    {
        var schedule = await _scheduleRepository.GetByIdWithSubclassesAsync(id);

        ArgumentNullException.ThrowIfNull(schedule, $"The schedule with id {id} was not found.");

        // Fetch only the required days from the database using their IDs
        var days = await _dayRepository.GetDaysAsync(scheduleDto.Days);

        // Assign the fetched days to the schedule
        schedule.Days.Clear();// Clear the old list of days in the memory
        schedule.Days.AddRange(days);

        await _scheduleRepository.UpdateAsync(schedule);

        _logger.LogInformation("Updated schedule with ID {id}", id);

        return schedule;
    }
}
