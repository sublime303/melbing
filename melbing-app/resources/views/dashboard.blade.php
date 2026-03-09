<!DOCTYPE html>
<html lang="en" class="h-full">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Melbing Ship Log — Dashboard</title>
    @vite(['resources/css/app.css', 'resources/js/app.js'])
    <style>
        body { background-color: #0a1628; color: #e2e8f0; }
        .card { background: linear-gradient(135deg, #0d2040 0%, #0f2d5a 100%); border: 1px solid #1e4d8c; border-radius: 0.75rem; }
        .stat-value { color: #38bdf8; font-size: 1.75rem; font-weight: 700; line-height: 1; }
        .stat-unit { color: #94a3b8; font-size: 0.8rem; font-weight: 500; }
        .stat-label { color: #94a3b8; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.05em; margin-top: 0.25rem; }
        .badge { padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; display: inline-flex; align-items: center; gap: 0.35rem; }
        .badge-on  { background: rgba(34,197,94,0.15); color: #4ade80; border: 1px solid rgba(34,197,94,0.4); }
        .badge-off { background: rgba(100,116,139,0.15); color: #94a3b8; border: 1px solid rgba(100,116,139,0.3); }
        .time-btn { background: #1a3d6e; color: #94a3b8; border: 1px solid #1e4d8c; padding: 0.3rem 0.9rem; border-radius: 0.375rem; font-size: 0.8rem; cursor: pointer; transition: all 0.2s; }
        .time-btn.active, .time-btn:hover { background: #1e4d8c; color: #38bdf8; border-color: #38bdf8; }
        .section-title { font-size: 0.7rem; text-transform: uppercase; letter-spacing: 0.1em; color: #c9a84c; font-weight: 600; }
        .no-data { color: #64748b; font-size: 0.9rem; margin-top: 0.25rem; }
        canvas { max-height: 240px; }
        ::-webkit-scrollbar { width: 6px; }
        ::-webkit-scrollbar-track { background: #0a1628; }
        ::-webkit-scrollbar-thumb { background: #1e4d8c; border-radius: 3px; }
    </style>
</head>
<body class="min-h-full">

    {{-- NAV --}}
    <nav style="background: linear-gradient(90deg, #0d2040, #0f2d5a); border-bottom: 1px solid #1e4d8c;">
        <div class="max-w-screen-2xl mx-auto px-4 sm:px-6 py-3 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-8 h-8" viewBox="0 0 64 64" fill="none">
                    <path d="M32 6 L32 44" stroke="#38bdf8" stroke-width="2.5" stroke-linecap="round"/>
                    <path d="M32 12 L50 32 L32 32 Z" fill="#38bdf8" opacity="0.8"/>
                    <path d="M32 18 L14 32 L32 32 Z" fill="#c9a84c" opacity="0.7"/>
                    <path d="M10 52 Q20 46 32 50 Q44 54 54 48" stroke="#38bdf8" stroke-width="2" fill="none" stroke-linecap="round"/>
                    <path d="M6 58 Q18 52 32 56 Q46 60 58 54" stroke="#1e4d8c" stroke-width="1.5" fill="none" stroke-linecap="round"/>
                </svg>
                <div>
                    <div class="font-bold text-lg text-white leading-none">Melbing</div>
                    <div class="text-xs" style="color:#c9a84c;">Ship Log Dashboard</div>
                </div>
            </div>
            <div class="flex items-center gap-4 text-sm" style="color:#94a3b8;">
                @if($latest)
                    <div class="flex items-center gap-2">
                        <span class="inline-block w-2 h-2 rounded-full bg-green-400 animate-pulse"></span>
                        <span>Last data: {{ \Carbon\Carbon::createFromTimestamp($latest->unix_time)->diffForHumans() }}</span>
                    </div>
                @else
                    <div class="flex items-center gap-2">
                        <span class="inline-block w-2 h-2 rounded-full bg-gray-500"></span>
                        <span>No data received</span>
                    </div>
                @endif
                <div>Records: <span class="text-white font-semibold">{{ number_format($totalRecords) }}</span></div>
            </div>
        </div>
    </nav>

    <div class="max-w-screen-2xl mx-auto px-4 sm:px-6 py-6 space-y-6">

        {{-- HEADER + TIME CONTROLS --}}
        <div class="flex items-center justify-between flex-wrap gap-3">
            <div>
                <div class="section-title mb-1">Current Readings</div>
                @if($latest)
                    <div class="text-white font-semibold">
                        {{ \Carbon\Carbon::createFromTimestamp($latest->unix_time)->format('D, d M Y H:i:s') }} UTC
                    </div>
                @else
                    <div class="no-data">No sensor data yet — waiting for ESP32...</div>
                @endif
            </div>
            <div class="flex gap-2">
                <button id="btn-24"  class="time-btn active" onclick="loadCharts(24)">24h</button>
                <button id="btn-168" class="time-btn"        onclick="loadCharts(168)">7d</button>
                <button id="btn-720" class="time-btn"        onclick="loadCharts(720)">30d</button>
            </div>
        </div>

        {{-- TEMPERATURE CARDS --}}
        <div>
            <div class="section-title mb-3">Temperatures</div>
            <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
                @foreach([
                    ['Inside',       '🏠', $latest?->inside_temp],
                    ['Outside',      '☁️', $latest?->outside_temp],
                    ['Water',        '🌊', $latest?->water_temp],
                    ['Refrigerator', '🧊', $latest?->refrigerator_temp],
                ] as [$label, $icon, $value])
                <div class="card p-4">
                    <div class="stat-label">{{ $icon }} {{ $label }}</div>
                    @if($value !== null)
                        <div class="stat-value mt-1">{{ number_format($value, 1) }}<span class="stat-unit ml-1">°C</span></div>
                    @else
                        <div class="no-data">—</div>
                    @endif
                </div>
                @endforeach
            </div>
        </div>

        {{-- ENVIRONMENT + BATTERIES + WIND --}}
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">

            <div class="card p-4 flex gap-6">
                <div class="flex-1">
                    <div class="stat-label">💧 Humidity</div>
                    @if($latest?->humidity !== null)
                        <div class="stat-value mt-1">{{ number_format($latest->humidity, 1) }}<span class="stat-unit ml-1">%</span></div>
                    @else
                        <div class="no-data">—</div>
                    @endif
                </div>
                <div class="flex-1">
                    <div class="stat-label">🌡 Pressure</div>
                    @if($latest?->pressure !== null)
                        <div class="stat-value mt-1" style="font-size:1.3rem;">{{ number_format($latest->pressure, 1) }}<span class="stat-unit ml-1">hPa</span></div>
                    @else
                        <div class="no-data">—</div>
                    @endif
                </div>
            </div>

            <div class="card p-4 flex gap-6">
                @foreach([
                    ['Consumer Bat.', '🔋', $latest?->consumer_bat_v],
                    ['Start Bat.',    '⚡', $latest?->start_bat_v],
                ] as [$lbl, $ico, $v])
                <div class="flex-1">
                    <div class="stat-label">{{ $ico }} {{ $lbl }}</div>
                    @if($v !== null)
                        @php $vc = $v >= 12.4 ? '#4ade80' : ($v >= 12.0 ? '#facc15' : '#f87171'); @endphp
                        <div class="stat-value mt-1" style="color:{{ $vc }}">{{ number_format($v, 2) }}<span class="stat-unit ml-1">V</span></div>
                    @else
                        <div class="no-data">—</div>
                    @endif
                </div>
                @endforeach
            </div>

            <div class="card p-4 flex gap-6">
                <div class="flex-1">
                    <div class="stat-label">💨 Wind Speed</div>
                    @if($latest?->wind_speed !== null)
                        <div class="stat-value mt-1">{{ number_format($latest->wind_speed, 1) }}<span class="stat-unit ml-1">m/s</span></div>
                    @else
                        <div class="no-data">—</div>
                    @endif
                </div>
                <div class="flex-1">
                    <div class="stat-label">🧭 True Wind</div>
                    @if($latest?->true_wind_angle !== null)
                        <div class="stat-value mt-1" style="font-size:1.4rem;">{{ number_format($latest->true_wind_angle, 0) }}<span class="stat-unit ml-1">°</span></div>
                    @else
                        <div class="no-data">—</div>
                    @endif
                </div>
            </div>

        </div>

        {{-- STATUS BADGES --}}
        <div class="card p-4">
            <div class="section-title mb-3">System Status</div>
            <div class="flex flex-wrap gap-3">
                @foreach([
                    ['Shore Power',     '🔌', $latest?->shore_power],
                    ['Battery Charger', '🔋', $latest?->battery_charger],
                    ['Solar Charger',   '☀️',  $latest?->solar_charger],
                    ['Heating Element', '🔥', $latest?->heating_element],
                    ['Dehumidifier',    '💨', $latest?->dehumidifier],
                    ['Daylight Saving', '🕐', $latest?->daylight_saving],
                ] as [$lbl, $ico, $val])
                    <span class="badge {{ $val ? 'badge-on' : 'badge-off' }}">
                        {{ $ico }} {{ $lbl }} &nbsp;{{ $val !== null ? ($val ? 'ON' : 'OFF') : '—' }}
                    </span>
                @endforeach
                @if($latest?->timezone)
                    <span class="badge" style="background:rgba(201,168,76,0.1);color:#c9a84c;border:1px solid rgba(201,168,76,0.3);">
                        🌍 {{ $latest->timezone }}
                    </span>
                @endif
            </div>
        </div>

        {{-- CHARTS --}}
        <div>
            <div class="section-title mb-3">Historical Charts</div>
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-4">
                <div class="card p-4">
                    <div class="text-sm font-semibold text-white mb-3 pb-2" style="border-bottom:1px solid #1e4d8c;">Temperature History</div>
                    <canvas id="chart-temp"></canvas>
                </div>
                <div class="card p-4">
                    <div class="text-sm font-semibold text-white mb-3 pb-2" style="border-bottom:1px solid #1e4d8c;">Atmospheric Pressure</div>
                    <canvas id="chart-pressure"></canvas>
                </div>
                <div class="card p-4">
                    <div class="text-sm font-semibold text-white mb-3 pb-2" style="border-bottom:1px solid #1e4d8c;">Battery Voltages</div>
                    <canvas id="chart-battery"></canvas>
                </div>
                <div class="card p-4">
                    <div class="text-sm font-semibold text-white mb-3 pb-2" style="border-bottom:1px solid #1e4d8c;">Wind Speed &amp; Angle</div>
                    <canvas id="chart-wind"></canvas>
                </div>
                <div class="card p-4 lg:col-span-2">
                    <div class="text-sm font-semibold text-white mb-3 pb-2" style="border-bottom:1px solid #1e4d8c;">Humidity</div>
                    <canvas id="chart-humidity"></canvas>
                </div>
            </div>
        </div>

        {{-- FOOTER --}}
        <div class="text-center text-xs pb-6" style="color:#334155;">
            Melbing Ship Log &mdash; ESP32-S3 Data Receiver
            @if($firstRecord)
                &mdash; Logging since {{ \Carbon\Carbon::createFromTimestamp($firstRecord->unix_time)->format('d M Y') }}
            @endif
        </div>

    </div>

<script>
let charts = {};
let currentHours = 24;

const SCALE_STYLE = {
    ticks: { color: '#64748b', font: { size: 10 }, maxTicksLimit: 8 },
    grid:  { color: 'rgba(30,77,140,0.3)' },
};

function formatLabel(ts) {
    const d = new Date(ts * 1000);
    return currentHours <= 24
        ? d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })
        : d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' });
}

function makeChart(id, labels, datasets, yLabel, y2Label) {
    const ctx = document.getElementById(id);
    if (!ctx) return;
    if (charts[id]) charts[id].destroy();

    const scales = {
        x: { ...SCALE_STYLE },
        y: { ...SCALE_STYLE, title: { display: !!yLabel, text: yLabel, color: '#64748b', font: { size: 10 } } },
    };
    if (y2Label) {
        scales.y2 = {
            position: 'right',
            ticks: { color: '#64748b', font: { size: 10 } },
            grid:  { drawOnChartArea: false },
            title: { display: true, text: y2Label, color: '#64748b', font: { size: 10 } },
        };
    }

    charts[id] = new Chart(ctx, {
        type: 'line',
        data: { labels, datasets: datasets.map(d => ({ pointRadius: 0, pointHoverRadius: 4, tension: 0.3, ...d })) },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: { labels: { color: '#94a3b8', font: { size: 11 }, boxWidth: 12, padding: 12 } },
                tooltip: { backgroundColor: '#0d2040', borderColor: '#1e4d8c', borderWidth: 1, titleColor: '#38bdf8', bodyColor: '#e2e8f0' },
            },
            scales,
        },
    });
}

async function loadCharts(hours) {
    currentHours = hours;
    document.querySelectorAll('.time-btn').forEach(b => b.classList.remove('active'));
    document.getElementById('btn-' + hours)?.classList.add('active');

    const res  = await fetch(`/api/logs?hours=${hours}`);
    const data = await res.json();

    if (!data.length) {
        Object.values(charts).forEach(c => c.destroy());
        charts = {};
        return;
    }

    const labels = data.map(r => formatLabel(r.unix_time));

    makeChart('chart-temp', labels, [
        { label: 'Inside °C',  data: data.map(r => r.inside_temp),       borderColor: '#f97316', backgroundColor: 'rgba(249,115,22,0.08)',   fill: false },
        { label: 'Outside °C', data: data.map(r => r.outside_temp),      borderColor: '#38bdf8', backgroundColor: 'rgba(56,189,248,0.08)',   fill: false },
        { label: 'Water °C',   data: data.map(r => r.water_temp),        borderColor: '#818cf8', backgroundColor: 'rgba(129,140,248,0.08)',  fill: false },
        { label: 'Fridge °C',  data: data.map(r => r.refrigerator_temp), borderColor: '#34d399', backgroundColor: 'rgba(52,211,153,0.08)',   fill: false },
    ], '°C');

    makeChart('chart-pressure', labels, [
        { label: 'Pressure hPa', data: data.map(r => r.pressure), borderColor: '#c9a84c', backgroundColor: 'rgba(201,168,76,0.12)', fill: true },
    ], 'hPa');

    makeChart('chart-battery', labels, [
        { label: 'Consumer Bat. V', data: data.map(r => r.consumer_bat_v), borderColor: '#4ade80', backgroundColor: 'rgba(74,222,128,0.08)',  fill: false },
        { label: 'Start Bat. V',    data: data.map(r => r.start_bat_v),    borderColor: '#facc15', backgroundColor: 'rgba(250,204,21,0.08)', fill: false },
    ], 'Volts');

    makeChart('chart-wind', labels, [
        { label: 'Wind Speed m/s',    data: data.map(r => r.wind_speed),      borderColor: '#38bdf8', backgroundColor: 'rgba(56,189,248,0.1)',  fill: true,  yAxisID: 'y' },
        { label: 'True Wind Angle °', data: data.map(r => r.true_wind_angle), borderColor: '#c9a84c', backgroundColor: 'rgba(201,168,76,0.05)', fill: false, yAxisID: 'y2' },
    ], 'm/s', '°');

    makeChart('chart-humidity', labels, [
        { label: 'Humidity %', data: data.map(r => r.humidity), borderColor: '#818cf8', backgroundColor: 'rgba(129,140,248,0.15)', fill: true },
    ], '%');
}

document.addEventListener('DOMContentLoaded', () => loadCharts(24));
setInterval(() => location.reload(), 60000);
</script>

</body>
</html>
