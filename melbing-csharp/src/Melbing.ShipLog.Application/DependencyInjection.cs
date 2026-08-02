using Melbing.ShipLog.Application.Abstractions;
using Melbing.ShipLog.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Melbing.ShipLog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IShipLogService, ShipLogService>();
        return services;
    }
}
