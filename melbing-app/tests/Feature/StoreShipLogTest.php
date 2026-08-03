<?php

namespace Tests\Feature;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;

class StoreShipLogTest extends TestCase
{
    use RefreshDatabase;

    public function test_it_stores_a_ship_log_and_returns_created(): void
    {
        $payload = [
            'unix_time' => now()->timestamp,
            'humidity' => 65.2,
            'inside_temp' => 23.27,
            'shore_power' => true,
            'timezone' => 'UTC+1',
        ];

        $response = $this->postJson(route('api.log.store'), $payload);

        $response->assertCreated()
            ->assertExactJson(['status' => 'ok']);

        $this->assertDatabaseHas('ship_logs', [
            'unix_time' => $payload['unix_time'],
            'humidity' => 65.2,
            'shore_power' => true,
            'timezone' => 'UTC+1',
        ]);
    }

    public function test_it_rejects_a_payload_missing_unix_time(): void
    {
        $response = $this->postJson(route('api.log.store'), [
            'humidity' => 50,
        ]);

        $response->assertUnprocessable()
            ->assertJsonValidationErrors(['unix_time']);

        $this->assertDatabaseCount('ship_logs', 0);
    }

    public function test_it_accepts_boolean_flags_as_integers(): void
    {
        $response = $this->postJson(route('api.log.store'), [
            'unix_time' => now()->timestamp,
            'shore_power' => 1,
            'heating_element' => 0,
        ]);

        $response->assertCreated();

        $this->assertDatabaseHas('ship_logs', [
            'shore_power' => true,
            'heating_element' => false,
        ]);
    }
}
