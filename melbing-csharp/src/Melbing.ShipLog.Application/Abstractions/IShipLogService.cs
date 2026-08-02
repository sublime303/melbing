using Melbing.ShipLog.Application.Contracts;

namespace Melbing.ShipLog.Application.Abstractions;

public interface IShipLogService
{
    Task<CreateShipLogResult> CreateAsync(
        ShipLogCreateRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipLogReadingDto>> GetRecentAsync(
        int hours = 24,
        CancellationToken cancellationToken = default);

    Task<DashboardSnapshot> GetDashboardAsync(CancellationToken cancellationToken = default);
}
