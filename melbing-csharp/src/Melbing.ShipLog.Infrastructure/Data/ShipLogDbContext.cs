using Microsoft.EntityFrameworkCore;
using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Infrastructure.Data;

public class ShipLogDbContext(DbContextOptions<ShipLogDbContext> options) : DbContext(options)
{
    public DbSet<ShipLogEntity> ShipLogs => Set<ShipLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShipLogEntity>(entity =>
        {
            entity.ToTable("ship_logs");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.UnixTime).HasColumnName("unix_time");
            entity.HasIndex(e => e.UnixTime);

            entity.Property(e => e.Humidity).HasColumnName("humidity");
            entity.Property(e => e.InsideTemp).HasColumnName("inside_temp");
            entity.Property(e => e.OutsideTemp).HasColumnName("outside_temp");
            entity.Property(e => e.WaterTemp).HasColumnName("water_temp");
            entity.Property(e => e.RefrigeratorTemp).HasColumnName("refrigerator_temp");
            entity.Property(e => e.Pressure).HasColumnName("pressure");
            entity.Property(e => e.ConsumerBatV).HasColumnName("consumer_bat_v");
            entity.Property(e => e.StartBatV).HasColumnName("start_bat_v");

            entity.Property(e => e.ShorePower).HasColumnName("shore_power").HasDefaultValue(false);
            entity.Property(e => e.WindSpeed).HasColumnName("wind_speed");
            entity.Property(e => e.RelativeWindAngle).HasColumnName("relative_wind_angle");
            entity.Property(e => e.TrueWindAngle).HasColumnName("true_wind_angle");

            entity.Property(e => e.HeatingElement).HasColumnName("heating_element").HasDefaultValue(false);
            entity.Property(e => e.Dehumidifier).HasColumnName("dehumidifier").HasDefaultValue(false);
            entity.Property(e => e.BatteryCharger).HasColumnName("battery_charger").HasDefaultValue(false);
            entity.Property(e => e.SolarCharger).HasColumnName("solar_charger").HasDefaultValue(false);
            entity.Property(e => e.DaylightSaving).HasColumnName("daylight_saving").HasDefaultValue(false);

            entity.Property(e => e.Timezone).HasColumnName("timezone").HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.Ignore(e => e.RecordedAt);
        });
    }
}
