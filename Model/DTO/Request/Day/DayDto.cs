namespace Entities.DTO.Request.Day;

public record DayDTO
{
    public required DateTime Day { get; init; }
    public required List<int> People { get; init; }
};