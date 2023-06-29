using Entities.DTO.Request.Day;

namespace Entities.DTO.Request.Schedule;

internal class ScheduleDto
{
    public required string Name { get; init; }
    public required List<DayDto> Days { get; init; }
}
