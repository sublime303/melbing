using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Melbing.ShipLog.Data;
using Melbing.ShipLog.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ShipLogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=Data/shiplog.db"));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "Data"));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShipLogDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.MapPost("/api/log", async (ShipLogCreateRequest request, ShipLogDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
    {
        return Results.ValidationProblem(
            validationResults
                .GroupBy(v => v.MemberNames.FirstOrDefault() ?? "")
                .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage ?? "Invalid").ToArray()));
    }

    var now = DateTime.UtcNow;
    var log = new Melbing.ShipLog.Models.ShipLog
    {
        UnixTime = request.UnixTime,
        Humidity = request.Humidity,
        InsideTemp = request.InsideTemp,
        OutsideTemp = request.OutsideTemp,
        WaterTemp = request.WaterTemp,
        RefrigeratorTemp = request.RefrigeratorTemp,
        Pressure = request.Pressure,
        ConsumerBatV = request.ConsumerBatV,
        StartBatV = request.StartBatV,
        ShorePower = request.ShorePower ?? false,
        WindSpeed = request.WindSpeed,
        RelativeWindAngle = request.RelativeWindAngle,
        TrueWindAngle = request.TrueWindAngle,
        HeatingElement = request.HeatingElement ?? false,
        Dehumidifier = request.Dehumidifier ?? false,
        BatteryCharger = request.BatteryCharger ?? false,
        SolarCharger = request.SolarCharger ?? false,
        DaylightSaving = request.DaylightSaving ?? false,
        Timezone = request.Timezone,
        CreatedAt = now,
        UpdatedAt = now,
    };

    db.ShipLogs.Add(log);
    await db.SaveChangesAsync();

    return Results.Json(new { status = "ok" }, statusCode: StatusCodes.Status201Created);
});

app.MapGet("/api/logs", async (ShipLogDbContext db, int hours = 24) =>
{
    hours = Math.Clamp(hours, 1, 720);
    var since = DateTimeOffset.UtcNow.AddHours(-hours).ToUnixTimeSeconds();

    var logs = await db.ShipLogs
        .AsNoTracking()
        .Where(l => l.UnixTime >= since)
        .OrderBy(l => l.UnixTime)
        .Select(l => new
        {
            unix_time = l.UnixTime,
            humidity = l.Humidity,
            inside_temp = l.InsideTemp,
            outside_temp = l.OutsideTemp,
            water_temp = l.WaterTemp,
            refrigerator_temp = l.RefrigeratorTemp,
            pressure = l.Pressure,
            consumer_bat_v = l.ConsumerBatV,
            start_bat_v = l.StartBatV,
            wind_speed = l.WindSpeed,
            relative_wind_angle = l.RelativeWindAngle,
            true_wind_angle = l.TrueWindAngle,
            shore_power = l.ShorePower,
            heating_element = l.HeatingElement,
            dehumidifier = l.Dehumidifier,
            battery_charger = l.BatteryCharger,
            solar_charger = l.SolarCharger,
        })
        .ToListAsync();

    return Results.Json(logs);
});

app.Run();

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
