using Melbing.ShipLog.Application.Abstractions;
using Melbing.ShipLog.Application.Contracts;
using Melbing.ShipLog.Application.Services;
using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Tests;

public class ShipLogServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsMappedEntity()
    {
        var repo = new FakeShipLogRepository();
        var service = new ShipLogService(repo);
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = await service.CreateAsync(new ShipLogCreateRequest
        {
            UnixTime = unixTime,
            Humidity = 50.5f,
            ShorePower = true,
            HeatingElement = false,
            Timezone = "UTC+1",
        });

        Assert.False(result.IsValidationError);
        var saved = Assert.Single(repo.Items);
        Assert.Equal(unixTime, saved.UnixTime);
        Assert.Equal(50.5f, saved.Humidity);
        Assert.True(saved.ShorePower);
        Assert.False(saved.HeatingElement);
        Assert.Equal("UTC+1", saved.Timezone);
        Assert.NotNull(saved.CreatedAt);
        Assert.Equal(1, repo.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationErrorWhenUnixTimeMissing()
    {
        var repo = new FakeShipLogRepository();
        var service = new ShipLogService(repo);

        // UnixTime defaults to 0; Required on value type still passes for 0.
        // Use MaxLength violation instead for a clear validation failure.
        var result = await service.CreateAsync(new ShipLogCreateRequest
        {
            UnixTime = 1,
            Timezone = new string('x', 51),
        });

        Assert.True(result.IsValidationError);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey(nameof(ShipLogCreateRequest.Timezone)));
        Assert.Empty(repo.Items);
        Assert.Equal(0, repo.SaveChangesCalls);
    }

    [Fact]
    public async Task GetRecentAsync_ClampsHoursBeforeQuerying()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var repo = new FakeShipLogRepository
        {
            Items =
            {
                new ShipLogEntity { UnixTime = now - 100 },
                new ShipLogEntity { UnixTime = now - 10_000_000 },
            },
        };
        var service = new ShipLogService(repo);

        var readings = await service.GetRecentAsync(hours: 9999);

        Assert.Equal(ShipLogService.MaxHours, repo.LastRequestedHoursEquivalent);
        Assert.Single(readings);
        Assert.Equal(now - 100, readings[0].UnixTime);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsLatestFirstAndCount()
    {
        var repo = new FakeShipLogRepository
        {
            Items =
            {
                new ShipLogEntity { UnixTime = 100, InsideTemp = 10 },
                new ShipLogEntity { UnixTime = 300, InsideTemp = 30 },
                new ShipLogEntity { UnixTime = 200, InsideTemp = 20 },
            },
        };
        var service = new ShipLogService(repo);

        var snapshot = await service.GetDashboardAsync();

        Assert.Equal(3, snapshot.TotalRecords);
        Assert.Equal(300, snapshot.Latest!.UnixTime);
        Assert.Equal(100, snapshot.FirstRecord!.UnixTime);
    }

    private sealed class FakeShipLogRepository : IShipLogRepository
    {
        public List<ShipLogEntity> Items { get; init; } = [];
        public int SaveChangesCalls { get; private set; }
        public int LastRequestedHoursEquivalent { get; private set; } = -1;

        public Task AddAsync(ShipLogEntity log, CancellationToken cancellationToken = default)
        {
            Items.Add(log);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ShipLogEntity>> GetSinceUnixTimeAsync(
            long sinceUnixTime,
            CancellationToken cancellationToken = default)
        {
            var hours = (int)Math.Round(
                (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - sinceUnixTime) / 3600.0);
            LastRequestedHoursEquivalent = hours;

            IReadOnlyList<ShipLogEntity> result = Items
                .Where(l => l.UnixTime >= sinceUnixTime)
                .OrderBy(l => l.UnixTime)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<ShipLogEntity?> GetLatestAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.OrderByDescending(l => l.UnixTime).FirstOrDefault());

        public Task<ShipLogEntity?> GetFirstAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.OrderBy(l => l.UnixTime).FirstOrDefault());

        public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.Count);
    }
}
