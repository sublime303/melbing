# Melbing Ship Log

A Laravel web application that receives sensor data from an ESP32-S3 on a sailing boat via HTTP POST requests, stores it in SQLite, and displays it on a live nautical dashboard.

## Tech Stack

- **PHP 8.5 / Laravel 12**
- **Laravel Boost** — project tooling & guidelines
- **SQLite** — database
- **Tailwind CSS v4** — styling
- **Chart.js** — historical charts
- **Vite** — frontend asset bundling

---

## Requirements

- PHP 8.2+
- Composer
- Node.js 18+ & npm

---

## Installation

```bash
# Install PHP dependencies
composer install

# Install frontend dependencies
npm install

# Copy environment file and generate app key
cp .env.example .env
php artisan key:generate

# Create the SQLite database and run migrations
touch database/database.sqlite
php artisan migrate

# Build frontend assets
npm run build
```

---

## Running the Development Server

```bash
php artisan serve
```

The dashboard will be available at **http://localhost:8000**.

For live frontend rebuilding during development:

```bash
npm run dev
```

---

## Dashboard

Open **http://localhost:8000** (or `/dashboard`) in a browser.

The dashboard displays:

- **Current readings** — the latest values for every sensor, timestamped
- **System status badges** — shore power, battery charger, solar charger, heating element, dehumidifier, daylight saving
- **Battery voltage** — colour-coded green (≥ 12.4 V), yellow (≥ 12.0 V), red (< 12.0 V)
- **Historical charts** — selectable time range: 24 h / 7 d / 30 d
  - Temperature (inside, outside, water, refrigerator)
  - Atmospheric pressure
  - Battery voltages
  - Wind speed & true wind angle
  - Humidity

The page auto-refreshes every 60 seconds.

---

## API Reference

### `POST /api/log` — Receive sensor data from ESP32

Accepts a JSON body with the sensor payload and stores it as a new `ShipLog` record.

**Headers**

```
Content-Type: application/json
Accept: application/json
```

**Request body fields**

| Field | Type | Unit | Required |
|---|---|---|---|
| `unix_time` | integer | seconds since epoch | yes |
| `humidity` | float | % | no |
| `inside_temp` | float | °C | no |
| `outside_temp` | float | °C | no |
| `water_temp` | float | °C | no |
| `refrigerator_temp` | float | °C | no |
| `pressure` | float | hPa | no |
| `consumer_bat_v` | float | V | no |
| `start_bat_v` | float | V | no |
| `shore_power` | boolean (0/1) | — | no |
| `wind_speed` | float | m/s | no |
| `relative_wind_angle` | float | ° | no |
| `true_wind_angle` | float | ° | no |
| `heating_element` | boolean (0/1) | — | no |
| `dehumidifier` | boolean (0/1) | — | no |
| `battery_charger` | boolean (0/1) | — | no |
| `solar_charger` | boolean (0/1) | — | no |
| `daylight_saving` | boolean (0/1) | — | no |
| `timezone` | string | e.g. `UTC+1` | no |

**Success response** `201 Created`

```json
{ "status": "ok" }
```

---

### `GET /api/logs` — Fetch historical records for charts

Returns an array of log records ordered by `unix_time` ascending, filtered to the last N hours.

**Query parameters**

| Parameter | Default | Max | Description |
|---|---|---|---|
| `hours` | `24` | `720` | How many hours of history to return |

**Example response**

```json
[
  {
    "unix_time": 1747840380,
    "humidity": 65.2,
    "inside_temp": 23.27,
    "outside_temp": 25.41,
    "water_temp": 14.34,
    "refrigerator_temp": 8.92,
    "pressure": 1013.25,
    "consumer_bat_v": 13.4,
    "start_bat_v": 14.2,
    "wind_speed": 3.6,
    "relative_wind_angle": 12.0,
    "true_wind_angle": 93.0,
    "shore_power": true,
    "heating_element": false,
    "dehumidifier": false,
    "battery_charger": true,
    "solar_charger": false
  }
]
```

---

## Testing with curl

### Send a single sensor reading

```bash
curl -X POST http://localhost:8000/api/log \
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

> `$(date +%s)` inserts the current Unix timestamp automatically. On Windows use a fixed integer instead.

Expected response:

```json
{"status":"ok"}
```

### Fetch the last 24 hours of data

```bash
curl "http://localhost:8000/api/logs?hours=24"
```

### Fetch the last 7 days of data

```bash
curl "http://localhost:8000/api/logs?hours=168"
```

### Send multiple test readings (simulate ESP32 history)

```bash
for i in $(seq 1 10); do
  curl -s -X POST http://localhost:8000/api/log \
    -H "Content-Type: application/json" \
    -H "Accept: application/json" \
    -d '{
      "unix_time": '"$(($(date +%s) - i * 3600))"',
      "humidity": '"$((60 + RANDOM % 20))"'.'"$((RANDOM % 10))"',
      "inside_temp": '"$((20 + RANDOM % 8))"'.'"$((RANDOM % 10))"',
      "outside_temp": '"$((15 + RANDOM % 12))"'.'"$((RANDOM % 10))"',
      "water_temp": '"$((12 + RANDOM % 6))"'.'"$((RANDOM % 10))"',
      "refrigerator_temp": '"$((5 + RANDOM % 5))"'.'"$((RANDOM % 10))"',
      "pressure": '"$((1005 + RANDOM % 20))"'.'"$((RANDOM % 100))"',
      "consumer_bat_v": '"$((12 + RANDOM % 3))"'.'"$((RANDOM % 10))"',
      "start_bat_v": '"$((12 + RANDOM % 3))"'.'"$((RANDOM % 10))"',
      "shore_power": 1,
      "wind_speed": '"$((RANDOM % 12))"'.'"$((RANDOM % 10))"',
      "relative_wind_angle": '"$((RANDOM % 360))"',
      "true_wind_angle": '"$((RANDOM % 360))"',
      "heating_element": 0,
      "dehumidifier": 0,
      "battery_charger": 1,
      "solar_charger": 1,
      "daylight_saving": 0,
      "timezone": "UTC+1"
    }'
  echo " → record $i sent"
done
```

---

## ESP32 Arduino sketch snippet

```cpp
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

const char* SERVER_URL = "http://your-server-ip:8000/api/log";

void sendSensorData(float humidity, float insideTemp, float pressure, float batV) {
    HTTPClient http;
    http.begin(SERVER_URL);
    http.addHeader("Content-Type", "application/json");
    http.addHeader("Accept", "application/json");

    JsonDocument doc;
    doc["unix_time"]      = (long)time(nullptr);
    doc["humidity"]       = humidity;
    doc["inside_temp"]    = insideTemp;
    doc["pressure"]       = pressure;
    doc["consumer_bat_v"] = batV;
    // ... add remaining fields

    String body;
    serializeJson(doc, body);

    int code = http.POST(body);
    http.end();
}
```

---

## Database

The app uses a single SQLite database at `database/database.sqlite`.

**Table: `ship_logs`**

| Column | Type | Notes |
|---|---|---|
| `id` | bigint | auto-increment PK |
| `unix_time` | bigint | indexed |
| `humidity` | float | nullable |
| `inside_temp` | float | nullable |
| `outside_temp` | float | nullable |
| `water_temp` | float | nullable |
| `refrigerator_temp` | float | nullable |
| `pressure` | float | nullable |
| `consumer_bat_v` | float | nullable |
| `start_bat_v` | float | nullable |
| `shore_power` | boolean | default false |
| `wind_speed` | float | nullable |
| `relative_wind_angle` | float | nullable |
| `true_wind_angle` | float | nullable |
| `heating_element` | boolean | default false |
| `dehumidifier` | boolean | default false |
| `battery_charger` | boolean | default false |
| `solar_charger` | boolean | default false |
| `daylight_saving` | boolean | default false |
| `timezone` | string | nullable |
| `created_at` | timestamp | |
| `updated_at` | timestamp | |

---

## Project Structure

```
melbing-app/
├── app/
│   ├── Http/Controllers/
│   │   ├── ApiController.php       # POST /api/log  &  GET /api/logs
│   │   └── DashboardController.php # GET /  and  GET /dashboard
│   └── Models/
│       └── ShipLog.php
├── database/
│   ├── migrations/
│   │   └── ..._create_ship_logs_table.php
│   └── database.sqlite
├── resources/
│   ├── css/app.css                 # Tailwind CSS
│   ├── js/app.js                   # Chart.js import
│   └── views/
│       └── dashboard.blade.php
├── routes/
│   ├── api.php                     # API routes
│   └── web.php                     # Dashboard route
└── bootstrap/app.php               # Routing + middleware config
```
