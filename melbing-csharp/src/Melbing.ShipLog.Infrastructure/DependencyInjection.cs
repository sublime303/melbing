using Melbing.ShipLog.Application.Abstractions;
using Melbing.ShipLog.Infrastructure.Data;
using Melbing.ShipLog.Infrastructure.Repositories;
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
