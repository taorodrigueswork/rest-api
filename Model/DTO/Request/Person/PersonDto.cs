using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Entities.DTO.Request.Person;

[ExcludeFromCodeCoverage]
public record PersonDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string Phone { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string Email { get; init; }
}
