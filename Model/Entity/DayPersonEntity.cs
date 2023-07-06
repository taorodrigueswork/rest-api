using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities.Entity;

/// <summary>
/// Create a new table for the many-to-many relationship between Day and Person.
/// </summary>
[Table("DayPerson")]
[ExcludeFromCodeCoverage]
public class DayPersonEntity
{
    public int DayId { get; set; }
    public int PersonId { get; set; }
}
