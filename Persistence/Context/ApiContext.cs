using Entities.Entity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Context;

[ExcludeFromCodeCoverage]
public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        // Configuring indirect many-to-many relationship between Day and Person using explicit class DayPersonEntity
        // check this link for more information: https://docs.microsoft.com/en-us/ef/core/modeling/relationships#many-to-many
        // https://www.thereformedprogrammer.net/updating-many-to-many-relationships-in-ef-core-5-and-above/
        modelBuilder.Entity<DayEntity>()
            .HasMany(n => n.People)
            .WithMany(n => n.Days)
            .UsingEntity<DayPersonEntity>(
                l => l.HasOne<PersonEntity>().WithMany().HasForeignKey(e => e.PersonId),
                r => r.HasOne<DayEntity>().WithMany().HasForeignKey(e => e.DayId));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }


    public DbSet<PersonEntity>? Person { get; set; }
    public DbSet<DayEntity>? Day { get; set; }
    public DbSet<ScheduleEntity>? Schedule { get; set; }
    public DbSet<DayPersonEntity>? DayPerson { get; set; }
}
