<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class ShipLog extends Model
{
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

    protected $casts = [
        'unix_time'           => 'integer',
        'humidity'            => 'float',
        'inside_temp'         => 'float',
        'outside_temp'        => 'float',
        'water_temp'          => 'float',
        'refrigerator_temp'   => 'float',
        'pressure'            => 'float',
        'consumer_bat_v'      => 'float',
        'start_bat_v'         => 'float',
        'shore_power'         => 'boolean',
        'wind_speed'          => 'float',
        'relative_wind_angle' => 'float',
        'true_wind_angle'     => 'float',
        'heating_element'     => 'boolean',
        'dehumidifier'        => 'boolean',
        'battery_charger'     => 'boolean',
        'solar_charger'       => 'boolean',
        'daylight_saving'     => 'boolean',
    ];

    public function getRecordedAtAttribute(): \Carbon\Carbon
    {
        return \Carbon\Carbon::createFromTimestamp($this->unix_time);
    }
}
