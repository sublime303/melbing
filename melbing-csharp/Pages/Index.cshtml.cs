using Melbing.ShipLog.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Melbing.ShipLog.Pages;

public class IndexModel(ShipLogDbContext db) : PageModel
{
    public Models.ShipLog? Latest { get; private set; }
    public Models.ShipLog? FirstRecord { get; private set; }
    public int TotalRecords { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        TotalRecords = await db.ShipLogs.CountAsync(cancellationToken);
        Latest = await db.ShipLogs
            .OrderByDescending(l => l.UnixTime)
            .FirstOrDefaultAsync(cancellationToken);
        FirstRecord = await db.ShipLogs
            .OrderBy(l => l.UnixTime)
            .FirstOrDefaultAsync(cancellationToken);
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
