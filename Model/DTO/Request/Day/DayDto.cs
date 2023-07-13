using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Entities.DTO.Request.Day;

[ExcludeFromCodeCoverage]
public record DayDto
{
    public required DateTime Day { get; init; }

    [Range(1, Int32.MaxValue)]
    public required int ScheduleId { get; init; }

    [MinLength(1)]
    public required List<int> People { get; init; }
};