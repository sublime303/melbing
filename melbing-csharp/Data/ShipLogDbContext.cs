using Melbing.ShipLog.Models;
using Microsoft.EntityFrameworkCore;

namespace Melbing.ShipLog.Data;

public class ShipLogDbContext(DbContextOptions<ShipLogDbContext> options) : DbContext(options)
{
    public DbSet<Models.ShipLog> ShipLogs => Set<Models.ShipLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.ShipLog>(entity =>
        {
            entity.HasIndex(e => e.UnixTime);
            entity.Property(e => e.ShorePower).HasDefaultValue(false);
            entity.Property(e => e.HeatingElement).HasDefaultValue(false);
            entity.Property(e => e.Dehumidifier).HasDefaultValue(false);
            entity.Property(e => e.BatteryCharger).HasDefaultValue(false);
            entity.Property(e => e.SolarCharger).HasDefaultValue(false);
            entity.Property(e => e.DaylightSaving).HasDefaultValue(false);
        });
    }
}
