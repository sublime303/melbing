using System.Text.Json.Serialization;

namespace Melbing.ShipLog.Domain.Dtos;

public sealed class ShipLogReadingDto
{
    [JsonPropertyName("unix_time")]
    public long UnixTime { get; init; }

    [JsonPropertyName("humidity")]
    public float? Humidity { get; init; }

    [JsonPropertyName("inside_temp")]
    public float? InsideTemp { get; init; }

    [JsonPropertyName("outside_temp")]
    public float? OutsideTemp { get; init; }

    [JsonPropertyName("water_temp")]
    public float? WaterTemp { get; init; }

    [JsonPropertyName("refrigerator_temp")]
    public float? RefrigeratorTemp { get; init; }

    [JsonPropertyName("pressure")]
    public float? Pressure { get; init; }

    [JsonPropertyName("consumer_bat_v")]
    public float? ConsumerBatV { get; init; }

    [JsonPropertyName("start_bat_v")]
    public float? StartBatV { get; init; }

    [JsonPropertyName("wind_speed")]
    public float? WindSpeed { get; init; }

    [JsonPropertyName("relative_wind_angle")]
    public float? RelativeWindAngle { get; init; }

    [JsonPropertyName("true_wind_angle")]
    public float? TrueWindAngle { get; init; }

    [JsonPropertyName("shore_power")]
    public bool ShorePower { get; init; }

    [JsonPropertyName("heating_element")]
    public bool HeatingElement { get; init; }

    [JsonPropertyName("dehumidifier")]
    public bool Dehumidifier { get; init; }

    [JsonPropertyName("battery_charger")]
    public bool BatteryCharger { get; init; }

    [JsonPropertyName("solar_charger")]
    public bool SolarCharger { get; init; }
}
