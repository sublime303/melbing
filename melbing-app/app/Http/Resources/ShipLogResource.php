<?php

namespace App\Http\Resources;

use Illuminate\Http\Request;
use Illuminate\Http\Resources\Json\JsonResource;

class ShipLogResource extends JsonResource
{
    /**
     * Transform the resource into an array.
     *
     * @return array<string, mixed>
     */
    public function toArray(Request $request): array
    {
        return [
            'unix_time' => $this->unix_time,
            'humidity' => $this->humidity,
            'inside_temp' => $this->inside_temp,
            'outside_temp' => $this->outside_temp,
            'water_temp' => $this->water_temp,
            'refrigerator_temp' => $this->refrigerator_temp,
            'pressure' => $this->pressure,
            'consumer_bat_v' => $this->consumer_bat_v,
            'start_bat_v' => $this->start_bat_v,
            'wind_speed' => $this->wind_speed,
            'relative_wind_angle' => $this->relative_wind_angle,
            'true_wind_angle' => $this->true_wind_angle,
            'shore_power' => $this->shore_power,
            'heating_element' => $this->heating_element,
            'dehumidifier' => $this->dehumidifier,
            'battery_charger' => $this->battery_charger,
            'solar_charger' => $this->solar_charger,
        ];
    }
}
