using Melbing.ShipLog.Application.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Web.Pages;

public class IndexModel(IShipLogService shipLogService) : PageModel
{
    public ShipLogEntity? Latest { get; private set; }
    public ShipLogEntity? FirstRecord { get; private set; }
    public int TotalRecords { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var snapshot = await shipLogService.GetDashboardAsync(cancellationToken);
        TotalRecords = snapshot.TotalRecords;
        Latest = snapshot.Latest;
        FirstRecord = snapshot.FirstRecord;
    }

    public static string FormatRelative(long unixTime)
    {
        var then = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        var span = DateTimeOffset.UtcNow - then;

        if (span.TotalSeconds < 60) return "just now";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} minutes ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
        if (span.TotalDays < 30) return $"{(int)span.TotalDays} days ago";
        return then.ToString("dd MMM yyyy");
    }

    public static string BatteryColor(float volts) =>
        volts >= 12.4f ? "#4ade80" : volts >= 12.0f ? "#facc15" : "#f87171";
}
