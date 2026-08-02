using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Melbing.ShipLog.Models;

[Table("ship_logs")]
public class ShipLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("unix_time")]
    [JsonPropertyName("unix_time")]
    public long UnixTime { get; set; }

    [Column("humidity")]
    [JsonPropertyName("humidity")]
    public float? Humidity { get; set; }

    [Column("inside_temp")]
    [JsonPropertyName("inside_temp")]
    public float? InsideTemp { get; set; }

    [Column("outside_temp")]
    [JsonPropertyName("outside_temp")]
    public float? OutsideTemp { get; set; }

    [Column("water_temp")]
    [JsonPropertyName("water_temp")]
    public float? WaterTemp { get; set; }

    [Column("refrigerator_temp")]
    [JsonPropertyName("refrigerator_temp")]
    public float? RefrigeratorTemp { get; set; }

    [Column("pressure")]
    [JsonPropertyName("pressure")]
    public float? Pressure { get; set; }

    [Column("consumer_bat_v")]
    [JsonPropertyName("consumer_bat_v")]
    public float? ConsumerBatV { get; set; }

    [Column("start_bat_v")]
    [JsonPropertyName("start_bat_v")]
    public float? StartBatV { get; set; }

    [Column("shore_power")]
    [JsonPropertyName("shore_power")]
    [JsonConverter(typeof(FlexibleBoolConverter))]
    public bool ShorePower { get; set; }

    [Column("wind_speed")]
    [JsonPropertyName("wind_speed")]
    public float? WindSpeed { get; set; }

    [Column("relative_wind_angle")]
    [JsonPropertyName("relative_wind_angle")]
    public float? RelativeWindAngle { get; set; }

    [Column("true_wind_angle")]
    [JsonPropertyName("true_wind_angle")]
    public float? TrueWindAngle { get; set; }

    [Column("heating_element")]
    [JsonPropertyName("heating_element")]
    [JsonConverter(typeof(FlexibleBoolConverter))]
    public bool HeatingElement { get; set; }

    [Column("dehumidifier")]
    [JsonPropertyName("dehumidifier")]
    [JsonConverter(typeof(FlexibleBoolConverter))]
    public bool Dehumidifier { get; set; }

    [Column("battery_charger")]
    [JsonPropertyName("battery_charger")]
    [JsonConverter(typeof(FlexibleBoolConverter))]
    public bool BatteryCharger { get; set; }

    [Column("solar_charger")]
    [JsonPropertyName("solar_charger")]
    [JsonConverter(typeof(FlexibleBoolConverter))]
    public bool SolarCharger { get; set; }

    [Column("daylight_saving")]
    [JsonPropertyName("daylight_saving")]
    [JsonConverter(typeof(FlexibleBoolConverter))]
    public bool DaylightSaving { get; set; }

    [Column("timezone")]
    [JsonPropertyName("timezone")]
    [MaxLength(50)]
    public string? Timezone { get; set; }

    [Column("created_at")]
    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    [JsonIgnore]
    public DateTimeOffset RecordedAt => DateTimeOffset.FromUnixTimeSeconds(UnixTime);
}
