# Melbing Ship Log (C# / ASP.NET Core)

Real-time sailing boat telemetry — ESP32 → ASP.NET Core → Dashboard.

This is a C# port of the Laravel app in `../melbing-app`, with the same HTTP API contract so the ESP32 firmware does not need to change.

```
┌──────────────────┐        HTTP POST        ┌──────────────────┐        ┌──────────────────┐
│                  │  ──── /api/log ────►    │                  │        │                  │
│   ESP32-S3       │                         │  ASP.NET Core    │ ──────►│    Dashboard     │
│   on the boat    │  ◄── {"status":"ok"} ── │  + SQLite DB     │        │  (browser)       │
│                  │                         │                  │        │                  │
└──────────────────┘                         └──────────────────┘        └──────────────────┘
```

## Solution structure

| Project | Responsibility |
|---|---|
| `src/Melbing.ShipLog.Domain` | Entities, DTOs |
| `src/Melbing.ShipLog.Application` | Service layer, interfaces, business logic |
| `src/Melbing.ShipLog.Infrastructure` | EF Core / SQLite, repository, JSON serialization adapters |
| `src/Melbing.ShipLog.Web` | Minimal APIs, Razor Pages, host / DI wiring |
| `tests/Melbing.ShipLog.Tests` | Unit + integration tests |

See [GUIDELINES.md](GUIDELINES.md) for Melbing’s layering and placement preferences.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

On Arch/CachyOS:

```bash
sudo pacman -S dotnet-sdk-8.0 aspnet-runtime-8.0 aspnet-targeting-pack-8.0
```

## Quick Start

```bash
cd melbing-csharp
dotnet run --project src/Melbing.ShipLog.Web
```

Open **http://localhost:5080** in your browser.

The SQLite database is created automatically at `src/Melbing.ShipLog.Web/Data/shiplog.db`.

## Tests

```bash
cd melbing-csharp
dotnet test
```

Integration tests cover `POST /api/log`, `GET /api/logs`, the dashboard page, ESP32-style bool parsing (`0`/`1`), and battery colour helpers. Unit tests cover `ShipLogService` with a fake repository.

## API

### Send a reading (ESP32 → server)

```bash
curl -X POST http://localhost:5080/api/log \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{
    "unix_time": '"$(date +%s)"',
    "humidity": 65.2,
    "inside_temp": 23.27,
    "outside_temp": 25.41,
    "water_temp": 14.34,
    "refrigerator_temp": 8.92,
    "pressure": 1013.25,
    "consumer_bat_v": 13.4,
    "start_bat_v": 14.2,
    "shore_power": 1,
    "wind_speed": 3.6,
    "relative_wind_angle": 12.0,
    "true_wind_angle": 93.0,
    "heating_element": 0,
    "dehumidifier": 0,
    "battery_charger": 1,
    "solar_charger": 0,
    "daylight_saving": 0,
    "timezone": "UTC+1"
  }'
```

### Fetch historical data (for charts)

```bash
curl "http://localhost:5080/api/logs?hours=24"
```

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 8 · ASP.NET Core (Minimal APIs + Razor Pages) |
| Database | SQLite via EF Core |
| Frontend | Tailwind CSS (CDN) · Chart.js 4 |
| Hardware | ESP32-S3 · ArduinoJson · HTTPClient |

## Notes

- Boolean fields accept `true`/`false`, `0`/`1`, or `"0"`/`"1"` (ESP32 / ArduinoJson style).
- Dashboard auto-refreshes every 60 seconds.
- Schema mirrors the Laravel `ship_logs` table (snake_case columns).
