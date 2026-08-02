using System.Net;
using System.Text;
using System.Text.Json;
using Melbing.ShipLog.Application.Serialization;
using Melbing.ShipLog.Web.Pages;

namespace Melbing.ShipLog.Tests;

public class FlexibleBoolConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new FlexibleBoolConverter(), new FlexibleNullableBoolConverter() },
    };

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("\"1\"", true)]
    [InlineData("\"0\"", false)]
    [InlineData("\"true\"", true)]
    [InlineData("\"false\"", false)]
    public void FlexibleBool_DeserializesEsp32Styles(string json, bool expected)
    {
        var value = JsonSerializer.Deserialize<bool>(json, Options);
        Assert.Equal(expected, value);
    }

    [Fact]
    public void FlexibleNullableBool_DeserializesNullAsNull()
    {
        var value = JsonSerializer.Deserialize<bool?>("null", Options);
        Assert.Null(value);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    public void FlexibleNullableBool_DeserializesNumeric(string json, bool expected)
    {
        var value = JsonSerializer.Deserialize<bool?>(json, Options);
        Assert.Equal(expected, value);
    }

    [Fact]
    public void FlexibleBool_RejectsUnsupportedToken()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<bool>("[1]", Options));
    }
}

public class DashboardTests : IClassFixture<MelbingWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly MelbingWebApplicationFactory _factory;

    public DashboardTests(MelbingWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Dashboard_ReturnsOkWithTitle()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Melbing Ship Log", html);
        Assert.Contains("Current Readings", html);
        Assert.Contains("chart-temp", html);
    }

    [Fact]
    public async Task Dashboard_ShowsLatestReadingWhenPresent()
    {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var payload = $$"""{"unix_time":{{unixTime}},"inside_temp":22.5,"shore_power":1}""";
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var post = await _client.PostAsync("/api/log", content);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("22.5", html);
        Assert.Contains("Shore Power", html);
        Assert.Contains("badge-on", html);
        Assert.Contains("Last data: just now", html);
    }
}

public class IndexModelHelperTests
{
    [Theory]
    [InlineData(12.4f, "#4ade80")]
    [InlineData(13.0f, "#4ade80")]
    [InlineData(12.0f, "#facc15")]
    [InlineData(12.3f, "#facc15")]
    [InlineData(11.9f, "#f87171")]
    public void BatteryColor_UsesVoltageThresholds(float volts, string expected)
    {
        Assert.Equal(expected, IndexModel.BatteryColor(volts));
    }

    [Fact]
    public void FormatRelative_ReturnsJustNowForRecentTimestamp()
    {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Assert.Equal("just now", IndexModel.FormatRelative(unixTime));
    }

    [Fact]
    public void FormatRelative_ReturnsMinutesAgo()
    {
        var unixTime = DateTimeOffset.UtcNow.AddMinutes(-15).ToUnixTimeSeconds();
        Assert.Equal("15 minutes ago", IndexModel.FormatRelative(unixTime));
    }
}
