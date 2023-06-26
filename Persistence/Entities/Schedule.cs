using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Entities;

[ExcludeFromCodeCoverage]
public class ScheduleEntity
{
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(256)")]
    public string Name { get; set; } = null!;

    [Required]
    public DateTime Created { get; set; } = DateTime.Now!;

    public virtual List<DayEntity> Days { get; set; } = new();
}
