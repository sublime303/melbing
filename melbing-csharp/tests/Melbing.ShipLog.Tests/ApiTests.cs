using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Melbing.ShipLog.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Melbing.ShipLog.Tests;

public class ApiTests : IClassFixture<MelbingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly MelbingWebApplicationFactory _factory;

    public ApiTests(MelbingWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostLog_WithEsp32Payload_ReturnsCreatedAndPersists()
    {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var payload = $$"""
            {
              "unix_time": {{unixTime}},
              "humidity": 65.2,
              "inside_temp": 23.27,
              "outside_temp": 25.41,
              "water_temp": 14.34,
              "refrigerator_temp": 8.92,
              "pressure": 1013.25,
              "consumer_bat_v": 13.4,
              "start_bat_v": 14.2,
              "shore_power": 1,
              "wind_speed": 3.6,
              "relative_wind_angle": 12.0,
              "true_wind_angle": 93.0,
              "heating_element": 0,
              "dehumidifier": 0,
              "battery_charger": 1,
              "solar_charger": 0,
              "daylight_saving": 0,
              "timezone": "UTC+1"
            }
            """;

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/log", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("ok", body.GetProperty("status").GetString());

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShipLogDbContext>();
        var saved = Assert.Single(db.ShipLogs.Where(l => l.UnixTime == unixTime));
        Assert.Equal(65.2f, saved.Humidity);
        Assert.True(saved.ShorePower);
        Assert.False(saved.HeatingElement);
        Assert.True(saved.BatteryCharger);
        Assert.Equal("UTC+1", saved.Timezone);
    }

    [Fact]
    public async Task PostLog_WithBooleanLiterals_AcceptsTrueFalse()
    {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
        var payload = $$"""
            {
              "unix_time": {{unixTime}},
              "shore_power": true,
              "heating_element": false
            }
            """;

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/log", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShipLogDbContext>();
        var saved = Assert.Single(db.ShipLogs.Where(l => l.UnixTime == unixTime));
        Assert.True(saved.ShorePower);
        Assert.False(saved.HeatingElement);
    }

    [Fact]
    public async Task GetLogs_ReturnsOnlyRecordsWithinHoursWindow()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await PostMinimalLogAsync(now - 3600);          // 1h ago — included
        await PostMinimalLogAsync(now - 48 * 3600);     // 48h ago — excluded for 24h window

        var response = await _client.GetAsync("/api/logs?hours=24");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var logs = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, logs.ValueKind);

        var times = logs.EnumerateArray()
            .Select(e => e.GetProperty("unix_time").GetInt64())
            .ToList();

        Assert.Contains(now - 3600, times);
        Assert.DoesNotContain(now - 48 * 3600, times);
    }

    [Fact]
    public async Task GetLogs_UsesSnakeCasePropertyNames()
    {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 5;
        await PostMinimalLogAsync(unixTime, humidity: 55.5f, insideTemp: 21.1f);

        var response = await _client.GetAsync("/api/logs?hours=1");
        var logs = await response.Content.ReadFromJsonAsync<JsonElement>();
        var entry = logs.EnumerateArray().First(e => e.GetProperty("unix_time").GetInt64() == unixTime);

        Assert.True(entry.TryGetProperty("unix_time", out _));
        Assert.True(entry.TryGetProperty("inside_temp", out _));
        Assert.True(entry.TryGetProperty("shore_power", out _));
        Assert.False(entry.TryGetProperty("UnixTime", out _));
        Assert.Equal(55.5, entry.GetProperty("humidity").GetDouble(), 1);
        Assert.Equal(21.1, entry.GetProperty("inside_temp").GetDouble(), 1);
    }

    [Fact]
    public async Task GetLogs_ClampsHoursToValidRange()
    {
        var response = await _client.GetAsync("/api/logs?hours=9999");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // 9999 is clamped to 720; endpoint should still succeed (empty or data).
        var logs = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, logs.ValueKind);
    }

    [Fact]
    public async Task GetLogs_OrdersByUnixTimeAscending()
    {
        var baseTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 100;
        await PostMinimalLogAsync(baseTime + 30);
        await PostMinimalLogAsync(baseTime + 10);
        await PostMinimalLogAsync(baseTime + 20);

        var response = await _client.GetAsync("/api/logs?hours=1");
        var logs = await response.Content.ReadFromJsonAsync<JsonElement>();
        var times = logs.EnumerateArray()
            .Select(e => e.GetProperty("unix_time").GetInt64())
            .Where(t => t >= baseTime && t <= baseTime + 30)
            .ToList();

        Assert.Equal(times.OrderBy(t => t), times);
    }

    private async Task PostMinimalLogAsync(long unixTime, float? humidity = null, float? insideTemp = null)
    {
        var payload = new Dictionary<string, object?>
        {
            ["unix_time"] = unixTime,
            ["humidity"] = humidity,
            ["inside_temp"] = insideTemp,
            ["shore_power"] = 0,
        };

        var response = await _client.PostAsJsonAsync("/api/log", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
