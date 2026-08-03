<?php

namespace App\Models;

use Carbon\Carbon;
use Database\Factories\ShipLogFactory;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class ShipLog extends Model
{
    /** @use HasFactory<ShipLogFactory> */
    use HasFactory;

    protected $fillable = [
        'unix_time',
        'humidity',
        'inside_temp',
        'outside_temp',
        'water_temp',
        'refrigerator_temp',
        'pressure',
        'consumer_bat_v',
        'start_bat_v',
        'shore_power',
        'wind_speed',
        'relative_wind_angle',
        'true_wind_angle',
        'heating_element',
        'dehumidifier',
        'battery_charger',
        'solar_charger',
        'daylight_saving',
        'timezone',
    ];

    /**
     * Get the attributes that should be cast.
     *
     * @return array<string, string>
     */
    protected function casts(): array
    {
        return [
            'unix_time' => 'integer',
            'humidity' => 'float',
            'inside_temp' => 'float',
            'outside_temp' => 'float',
            'water_temp' => 'float',
            'refrigerator_temp' => 'float',
            'pressure' => 'float',
            'consumer_bat_v' => 'float',
            'start_bat_v' => 'float',
            'shore_power' => 'boolean',
            'wind_speed' => 'float',
            'relative_wind_angle' => 'float',
            'true_wind_angle' => 'float',
            'heating_element' => 'boolean',
            'dehumidifier' => 'boolean',
            'battery_charger' => 'boolean',
            'solar_charger' => 'boolean',
            'daylight_saving' => 'boolean',
        ];
    }

    public function getRecordedAtAttribute(): Carbon
    {
        return Carbon::createFromTimestamp($this->unix_time);
    }
}
