using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

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

        modelBuilder.Entity<DayEntity>()
            .HasMany(n => n.People)
            .WithMany(n => n.Days)
            .UsingEntity<DayPersonEntity>(
                l => l.HasOne<PersonEntity>().WithMany().HasForeignKey(e => e.PersonId),
                r => r.HasOne<DayEntity>().WithMany().HasForeignKey(e => e.DayId));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseChangeTrackingProxies(false, false);
    }

    public DbSet<PersonEntity>? People { get; set; }
    public DbSet<DayEntity>? Days { get; set; }
    public DbSet<ScheduleEntity>? Schedules { get; set; }
}
