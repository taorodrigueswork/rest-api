namespace Entities.DTO.Request.Schedule;

public record ScheduleDto
{
    public required string Name { get; init; }
    public required List<int> Days { get; init; }
}
