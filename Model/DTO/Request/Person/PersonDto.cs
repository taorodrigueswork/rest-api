namespace Entities.DTO.Request.Person;

public record PersonDto
{
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
}
