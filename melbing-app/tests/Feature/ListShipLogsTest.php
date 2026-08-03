<?php

namespace Tests\Feature;

use App\Models\ShipLog;
use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;

class ListShipLogsTest extends TestCase
{
    use RefreshDatabase;

    public function test_it_returns_logs_within_the_requested_hours_window(): void
    {
        $recent = ShipLog::factory()->create([
            'unix_time' => now()->subHours(2)->timestamp,
            'inside_temp' => 21.5,
        ]);

        ShipLog::factory()->create([
            'unix_time' => now()->subHours(48)->timestamp,
            'inside_temp' => 10.0,
        ]);

        $response = $this->getJson(route('api.logs.index', ['hours' => 24]));

        $response->assertOk()
            ->assertJsonCount(1)
            ->assertJsonPath('0.unix_time', $recent->unix_time)
            ->assertJsonPath('0.inside_temp', 21.5)
            ->assertJsonStructure([
                '*' => [
                    'unix_time',
                    'humidity',
                    'inside_temp',
                    'outside_temp',
                    'water_temp',
                    'refrigerator_temp',
                    'pressure',
                    'consumer_bat_v',
                    'start_bat_v',
                    'wind_speed',
                    'relative_wind_angle',
                    'true_wind_angle',
                    'shore_power',
                    'heating_element',
                    'dehumidifier',
                    'battery_charger',
                    'solar_charger',
                ],
            ]);
    }

    public function test_it_defaults_to_twenty_four_hours(): void
    {
        ShipLog::factory()->create([
            'unix_time' => now()->subHours(12)->timestamp,
        ]);

        ShipLog::factory()->create([
            'unix_time' => now()->subHours(30)->timestamp,
        ]);

        $this->getJson(route('api.logs.index'))
            ->assertOk()
            ->assertJsonCount(1);
    }

    public function test_it_rejects_hours_outside_allowed_bounds(): void
    {
        $this->getJson(route('api.logs.index', ['hours' => 0]))
            ->assertUnprocessable()
            ->assertJsonValidationErrors(['hours']);

        $this->getJson(route('api.logs.index', ['hours' => 721]))
            ->assertUnprocessable()
            ->assertJsonValidationErrors(['hours']);
    }
}
