namespace Melbing.ShipLog.Domain.Entities;

public class ShipLog
{
    public long Id { get; set; }

    public long UnixTime { get; set; }

    public float? Humidity { get; set; }

    public float? InsideTemp { get; set; }

    public float? OutsideTemp { get; set; }

    public float? WaterTemp { get; set; }

    public float? RefrigeratorTemp { get; set; }

    public float? Pressure { get; set; }

    public float? ConsumerBatV { get; set; }

    public float? StartBatV { get; set; }

    public bool ShorePower { get; set; }

    public float? WindSpeed { get; set; }

    public float? RelativeWindAngle { get; set; }

    public float? TrueWindAngle { get; set; }

    public bool HeatingElement { get; set; }

    public bool Dehumidifier { get; set; }

    public bool BatteryCharger { get; set; }

    public bool SolarCharger { get; set; }

    public bool DaylightSaving { get; set; }

    public string? Timezone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTimeOffset RecordedAt => DateTimeOffset.FromUnixTimeSeconds(UnixTime);
}
