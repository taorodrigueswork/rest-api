using System.Diagnostics.CodeAnalysis;

namespace Entities.DTO.Request.Schedule;

[ExcludeFromCodeCoverage]
public record ScheduleDto
{
    public required string Name { get; init; }
    public required List<int> Days { get; init; }
}
