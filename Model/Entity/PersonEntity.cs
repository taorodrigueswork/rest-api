using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities.Entity;

[ExcludeFromCodeCoverage]
public class PersonEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(256)")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "varchar(128)")]
    public string Phone { get; set; } = null!;

    [Column(TypeName = "varchar(128)")]
    public string Email { get; set; } = null!;

    public virtual List<DayEntity> Days { get; set; } = new();
}
