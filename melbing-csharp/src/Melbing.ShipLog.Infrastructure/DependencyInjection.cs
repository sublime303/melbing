using System.Text.Json;
using Melbing.ShipLog.Application.Interfaces;
using Melbing.ShipLog.Infrastructure.Data;
using Melbing.ShipLog.Infrastructure.Repositories;
using Melbing.ShipLog.Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Melbing.ShipLog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? "Data Source=Data/shiplog.db";

        services.AddDbContext<ShipLogDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IShipLogRepository, ShipLogRepository>();

        return services;
    }
}

public static class JsonSerializerConfiguration
{
    /// <summary>
    /// Registers ESP32-compatible JSON converters (flexible bools as 0/1).
    /// </summary>
    public static JsonSerializerOptions AddEsp32Converters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new FlexibleBoolConverter());
        options.Converters.Add(new FlexibleNullableBoolConverter());
        return options;
    }
}
