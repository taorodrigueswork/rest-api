using System.Diagnostics.CodeAnalysis;

namespace Entities.DTO.Request.Day;

[ExcludeFromCodeCoverage]
public record DayDto
{
    public required DateTime Day { get; init; }
    public required int ScheduleId { get; init; }
    public required List<int> People { get; init; }
};