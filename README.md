<div align="center">

<!-- Sailboat ASCII art as a header banner -->

```
        |    |    |
       )_)  )_)  )_)
      )___))___))___)\\
     )____)____)_____)\\
   _____|____|____|____\\\__
  \                   /
~~~^~^~^~^~^~^~^~^~^~^~^~^~~
```

# ⚓ Melbing Ship Log

**Real-time sailing boat telemetry — ESP32 → Laravel → Dashboard**

[![PHP](https://img.shields.io/badge/PHP-8.5-777BB4?style=flat-square&logo=php&logoColor=white)](https://php.net)
[![Laravel](https://img.shields.io/badge/Laravel-12-FF2D20?style=flat-square&logo=laravel&logoColor=white)](https://laravel.com)
[![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white)](https://sqlite.org)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-v4-38BDF8?style=flat-square&logo=tailwindcss&logoColor=white)](https://tailwindcss.com)
[![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?style=flat-square&logo=chart.js&logoColor=white)](https://www.chartjs.org)

</div>

---

## What is this?

Melbing is a self-hosted telemetry server for a sailing boat. An **ESP32-S3** microcontroller on board reads sensors and sends data to this Laravel web server via HTTP POST requests. The server stores everything in a SQLite database and displays it on a live, dark-themed nautical dashboard.

```
┌──────────────────┐        HTTP POST        ┌──────────────────┐        ┌──────────────────┐
│                  │  ──── /api/log ────►    │                  │        │                  │
│   ESP32-S3       │                         │  Laravel Server  │ ──────►│    Dashboard     │
│   on the boat    │  ◄── {"status":"ok"} ── │  + SQLite DB     │        │  (browser)       │
│                  │                         │                  │        │                  │
└──────────────────┘                         └──────────────────┘        └──────────────────┘
```

---

## Sensors & Data

The ESP32 transmits the following fields with every reading:

| Category | Field | Unit |
|---|---|---|
| **Time** | `unix_time` | Unix timestamp |
| **Climate** | `humidity`, `pressure` | %, hPa |
| **Temperature** | `inside_temp`, `outside_temp`, `water_temp`, `refrigerator_temp` | °C |
| **Electrical** | `consumer_bat_v`, `start_bat_v` | Volts |
| **Wind** | `wind_speed`, `relative_wind_angle`, `true_wind_angle` | m/s, ° |
| **Status** | `shore_power`, `battery_charger`, `solar_charger`, `heating_element`, `dehumidifier` | on/off |

---

## Dashboard

A dark, nautical-themed dashboard served at `/` showing:

- **Current readings** for all sensors with live timestamp
- **Colour-coded battery voltage** — green (≥ 12.4 V) · yellow (≥ 12.0 V) · red (< 12.0 V)
- **System status badges** — shore power, solar charger, battery charger, heating, dehumidifier
- **5 historical charts** with 24 h / 7 d / 30 d time-range toggle:
  - Temperature history (inside, outside, water, fridge)
  - Atmospheric pressure
  - Battery voltages
  - Wind speed & true wind angle
  - Humidity
- **Auto-refreshes** every 60 seconds

---

## Quick Start

```bash
cd melbing-app

composer install
npm install

cp .env.example .env
php artisan key:generate

touch database/database.sqlite
php artisan migrate

npm run build
php artisan serve
```

Open **http://localhost:8000** in your browser.

---

## API — Quick Reference

### Send a reading (ESP32 → server)

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

### Fetch historical data (for charts)

```bash
curl "http://localhost:8000/api/logs?hours=24"
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | PHP 8.5 · Laravel 12 · Laravel Boost |
| Database | SQLite |
| Frontend | Tailwind CSS v4 · Chart.js · Vite |
| Hardware | ESP32-S3 · ArduinoJson · HTTPClient |

---

## Full Documentation

Detailed installation instructions, full API reference, database schema, curl test scripts, and an Arduino sketch snippet are in the app README:

**[→ melbing-app/README.md](melbing-app/README.md)**

---

<div align="center">
  <sub>Built for <em>M/S Melbing</em> &nbsp;⚓&nbsp; MIT License</sub>
</div>
