namespace Entities.DTO.Request.Person;

public record PersonDTO
{
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
}
