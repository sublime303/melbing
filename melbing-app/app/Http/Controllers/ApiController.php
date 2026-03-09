<?php

namespace App\Http\Controllers;

use App\Models\ShipLog;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;

class ApiController extends Controller
{
    public function store(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'unix_time'           => 'required|integer',
            'humidity'            => 'nullable|numeric',
            'inside_temp'         => 'nullable|numeric',
            'outside_temp'        => 'nullable|numeric',
            'water_temp'          => 'nullable|numeric',
            'refrigerator_temp'   => 'nullable|numeric',
            'pressure'            => 'nullable|numeric',
            'consumer_bat_v'      => 'nullable|numeric',
            'start_bat_v'         => 'nullable|numeric',
            'shore_power'         => 'nullable|boolean',
            'wind_speed'          => 'nullable|numeric',
            'relative_wind_angle' => 'nullable|numeric',
            'true_wind_angle'     => 'nullable|numeric',
            'heating_element'     => 'nullable|boolean',
            'dehumidifier'        => 'nullable|boolean',
            'battery_charger'     => 'nullable|boolean',
            'solar_charger'       => 'nullable|boolean',
            'daylight_saving'     => 'nullable|boolean',
            'timezone'            => 'nullable|string|max:50',
        ]);

        ShipLog::create($validated);

        return response()->json(['status' => 'ok'], 201);
    }

    public function index(Request $request): JsonResponse
    {
        $hours = (int) $request->query('hours', 24);
        $hours = min(max($hours, 1), 720);

        $since = now()->subHours($hours)->timestamp;

        $logs = ShipLog::where('unix_time', '>=', $since)
            ->orderBy('unix_time')
            ->get([
                'unix_time', 'humidity', 'inside_temp', 'outside_temp',
                'water_temp', 'refrigerator_temp', 'pressure',
                'consumer_bat_v', 'start_bat_v', 'wind_speed',
                'relative_wind_angle', 'true_wind_angle',
                'shore_power', 'heating_element', 'dehumidifier',
                'battery_charger', 'solar_charger',
            ]);

        return response()->json($logs);
    }
}
