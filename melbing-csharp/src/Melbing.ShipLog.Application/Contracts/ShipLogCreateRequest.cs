using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Melbing.ShipLog.Application.Serialization;

namespace Melbing.ShipLog.Application.Contracts;

public sealed class ShipLogCreateRequest
{
    [Required]
    [JsonPropertyName("unix_time")]
    public long UnixTime { get; set; }

    [JsonPropertyName("humidity")]
    public float? Humidity { get; set; }

    [JsonPropertyName("inside_temp")]
    public float? InsideTemp { get; set; }

    [JsonPropertyName("outside_temp")]
    public float? OutsideTemp { get; set; }

    [JsonPropertyName("water_temp")]
    public float? WaterTemp { get; set; }

    [JsonPropertyName("refrigerator_temp")]
    public float? RefrigeratorTemp { get; set; }

    [JsonPropertyName("pressure")]
    public float? Pressure { get; set; }

    [JsonPropertyName("consumer_bat_v")]
    public float? ConsumerBatV { get; set; }

    [JsonPropertyName("start_bat_v")]
    public float? StartBatV { get; set; }

    [JsonPropertyName("shore_power")]
    [JsonConverter(typeof(FlexibleNullableBoolConverter))]
    public bool? ShorePower { get; set; }

    [JsonPropertyName("wind_speed")]
    public float? WindSpeed { get; set; }

    [JsonPropertyName("relative_wind_angle")]
    public float? RelativeWindAngle { get; set; }

    [JsonPropertyName("true_wind_angle")]
    public float? TrueWindAngle { get; set; }

    [JsonPropertyName("heating_element")]
    [JsonConverter(typeof(FlexibleNullableBoolConverter))]
    public bool? HeatingElement { get; set; }

    [JsonPropertyName("dehumidifier")]
    [JsonConverter(typeof(FlexibleNullableBoolConverter))]
    public bool? Dehumidifier { get; set; }

    [JsonPropertyName("battery_charger")]
    [JsonConverter(typeof(FlexibleNullableBoolConverter))]
    public bool? BatteryCharger { get; set; }

    [JsonPropertyName("solar_charger")]
    [JsonConverter(typeof(FlexibleNullableBoolConverter))]
    public bool? SolarCharger { get; set; }

    [JsonPropertyName("daylight_saving")]
    [JsonConverter(typeof(FlexibleNullableBoolConverter))]
    public bool? DaylightSaving { get; set; }

    [MaxLength(50)]
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}
