using System.ComponentModel.DataAnnotations;
using Melbing.ShipLog.Application.Interfaces;
using Melbing.ShipLog.Domain.Dtos;
using ShipLogEntity = Melbing.ShipLog.Domain.Entities.ShipLog;

namespace Melbing.ShipLog.Application.Service;

public sealed class ShipLogService(IShipLogRepository repository) : IShipLogService
{
    public const int MinHours = 1;
    public const int MaxHours = 720;

    public async Task<CreateShipLogResult> CreateAsync(
        ShipLogCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
        {
            var errors = validationResults
                .GroupBy(v => v.MemberNames.FirstOrDefault() ?? "")
                .ToDictionary(g => g.Key, g => g.Select(v => v.ErrorMessage ?? "Invalid").ToArray());
            return CreateShipLogResult.ValidationError(errors);
        }

        var now = DateTime.UtcNow;
        var log = new ShipLogEntity
        {
            UnixTime = request.UnixTime,
            Humidity = request.Humidity,
            InsideTemp = request.InsideTemp,
            OutsideTemp = request.OutsideTemp,
            WaterTemp = request.WaterTemp,
            RefrigeratorTemp = request.RefrigeratorTemp,
            Pressure = request.Pressure,
            ConsumerBatV = request.ConsumerBatV,
            StartBatV = request.StartBatV,
            ShorePower = request.ShorePower ?? false,
            WindSpeed = request.WindSpeed,
            RelativeWindAngle = request.RelativeWindAngle,
            TrueWindAngle = request.TrueWindAngle,
            HeatingElement = request.HeatingElement ?? false,
            Dehumidifier = request.Dehumidifier ?? false,
            BatteryCharger = request.BatteryCharger ?? false,
            SolarCharger = request.SolarCharger ?? false,
            DaylightSaving = request.DaylightSaving ?? false,
            Timezone = request.Timezone,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await repository.AddAsync(log, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return CreateShipLogResult.Success();
    }

    public async Task<IReadOnlyList<ShipLogReadingDto>> GetRecentAsync(
        int hours = 24,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, MinHours, MaxHours);
        var since = DateTimeOffset.UtcNow.AddHours(-hours).ToUnixTimeSeconds();

        var logs = await repository.GetSinceUnixTimeAsync(since, cancellationToken);

        return logs
            .Select(l => new ShipLogReadingDto
            {
                UnixTime = l.UnixTime,
                Humidity = l.Humidity,
                InsideTemp = l.InsideTemp,
                OutsideTemp = l.OutsideTemp,
                WaterTemp = l.WaterTemp,
                RefrigeratorTemp = l.RefrigeratorTemp,
                Pressure = l.Pressure,
                ConsumerBatV = l.ConsumerBatV,
                StartBatV = l.StartBatV,
                WindSpeed = l.WindSpeed,
                RelativeWindAngle = l.RelativeWindAngle,
                TrueWindAngle = l.TrueWindAngle,
                ShorePower = l.ShorePower,
                HeatingElement = l.HeatingElement,
                Dehumidifier = l.Dehumidifier,
                BatteryCharger = l.BatteryCharger,
                SolarCharger = l.SolarCharger,
            })
            .ToList();
    }

    public async Task<DashboardSnapshot> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return new DashboardSnapshot
        {
            TotalRecords = await repository.CountAsync(cancellationToken),
            Latest = await repository.GetLatestAsync(cancellationToken),
            FirstRecord = await repository.GetFirstAsync(cancellationToken),
        };
    }
}
