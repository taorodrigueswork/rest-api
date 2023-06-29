using Entities.DTO.Request.Day;

namespace Entities.DTO.Request.Schedule;

internal class ScheduleDTO
{
    public required string Name { get; init; }
    public required List<DayDTO> Days { get; init; }
}
