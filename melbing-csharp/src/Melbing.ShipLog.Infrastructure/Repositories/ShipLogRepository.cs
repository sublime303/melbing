using Melbing.ShipLog.Application.Interfaces;
using Melbing.ShipLog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Infrastructure.Repositories;

public sealed class ShipLogRepository(ShipLogDbContext db) : IShipLogRepository
{
    public Task AddAsync(ShipLogEntity log, CancellationToken cancellationToken = default)
    {
        db.ShipLogs.Add(log);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public async Task<IReadOnlyList<ShipLogEntity>> GetSinceUnixTimeAsync(
        long sinceUnixTime,
        CancellationToken cancellationToken = default)
    {
        return await db.ShipLogs
            .AsNoTracking()
            .Where(l => l.UnixTime >= sinceUnixTime)
            .OrderBy(l => l.UnixTime)
            .ToListAsync(cancellationToken);
    }

    public Task<ShipLogEntity?> GetLatestAsync(CancellationToken cancellationToken = default) =>
        db.ShipLogs
            .AsNoTracking()
            .OrderByDescending(l => l.UnixTime)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<ShipLogEntity?> GetFirstAsync(CancellationToken cancellationToken = default) =>
        db.ShipLogs
            .AsNoTracking()
            .OrderBy(l => l.UnixTime)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        db.ShipLogs.CountAsync(cancellationToken);
}
