using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity;

/// <summary>
/// Create a new table for the many-to-many relationship between Day and Person.
/// </summary>
[Table("DayPerson")]
public class DayPersonEntity
{
    public int DayId { get; set; }
    public int PersonId { get; set; }
}
