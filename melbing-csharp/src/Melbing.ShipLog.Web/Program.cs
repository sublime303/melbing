using System.Globalization;
using System.Text.Json.Serialization;
using Melbing.ShipLog.Application;
using Melbing.ShipLog.Application.Abstractions;
using Melbing.ShipLog.Application.Contracts;
using Melbing.ShipLog.Infrastructure;
using Melbing.ShipLog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.MapPost("/api/log", async (ShipLogCreateRequest request, IShipLogService service) =>
{
    var result = await service.CreateAsync(request);
    return result.IsValidationError
        ? Results.ValidationProblem(result.Errors!)
        : Results.Json(new { status = "ok" }, statusCode: StatusCodes.Status201Created);
});

app.MapGet("/api/logs", async (IShipLogService service, int hours = 24) =>
    Results.Json(await service.GetRecentAsync(hours)));

app.Run();

// Expose entry point assembly to WebApplicationFactory in tests.
public partial class Program;
