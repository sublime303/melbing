using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Application.Contracts;

public sealed class DashboardSnapshot
{
    public ShipLogEntity? Latest { get; init; }
    public ShipLogEntity? FirstRecord { get; init; }
    public int TotalRecords { get; init; }
}
