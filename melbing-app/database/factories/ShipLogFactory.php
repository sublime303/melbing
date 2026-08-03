<?php

namespace Database\Factories;

use App\Models\ShipLog;
use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends Factory<ShipLog>
 */
class ShipLogFactory extends Factory
{
    protected $model = ShipLog::class;

    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        return [
            'unix_time' => now()->subMinutes(fake()->numberBetween(0, 1440))->timestamp,
            'humidity' => fake()->randomFloat(1, 40, 90),
            'inside_temp' => fake()->randomFloat(1, 15, 30),
            'outside_temp' => fake()->randomFloat(1, 5, 35),
            'water_temp' => fake()->randomFloat(1, 8, 22),
            'refrigerator_temp' => fake()->randomFloat(1, 2, 12),
            'pressure' => fake()->randomFloat(2, 990, 1030),
            'consumer_bat_v' => fake()->randomFloat(1, 11.5, 14.5),
            'start_bat_v' => fake()->randomFloat(1, 11.5, 14.5),
            'shore_power' => fake()->boolean(),
            'wind_speed' => fake()->randomFloat(1, 0, 20),
            'relative_wind_angle' => fake()->randomFloat(1, 0, 360),
            'true_wind_angle' => fake()->randomFloat(1, 0, 360),
            'heating_element' => fake()->boolean(),
            'dehumidifier' => fake()->boolean(),
            'battery_charger' => fake()->boolean(),
            'solar_charger' => fake()->boolean(),
            'daylight_saving' => fake()->boolean(),
            'timezone' => 'UTC+1',
        ];
    }
}
