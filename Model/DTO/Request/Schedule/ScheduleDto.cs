namespace Entities.DTO.Request.Schedule;

public record ScheduleDTO
{
    public required string Name { get; init; }
    public required List<int> Days { get; init; }
}
