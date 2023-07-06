using System.Diagnostics.CodeAnalysis;

namespace Entities.DTO.Request.Person;

[ExcludeFromCodeCoverage]
public record PersonDto
{
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
}
