using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Application.Interfaces;

public interface IShipLogRepository
{
    Task AddAsync(ShipLogEntity log, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipLogEntity>> GetSinceUnixTimeAsync(
        long sinceUnixTime,
        CancellationToken cancellationToken = default);

    Task<ShipLogEntity?> GetLatestAsync(CancellationToken cancellationToken = default);

    Task<ShipLogEntity?> GetFirstAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
