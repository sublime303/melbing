<?php

namespace Tests\Feature;

use App\Models\ShipLog;
use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;

class DashboardTest extends TestCase
{
    use RefreshDatabase;

    public function test_dashboard_returns_successful_response_with_ship_log_data(): void
    {
        ShipLog::factory()->create([
            'unix_time' => now()->timestamp,
            'inside_temp' => 22.5,
            'consumer_bat_v' => 13.1,
        ]);

        $this->get('/')
            ->assertOk()
            ->assertViewIs('dashboard')
            ->assertViewHas('latest')
            ->assertViewHas('totalRecords', 1);

        $this->get(route('dashboard'))
            ->assertOk()
            ->assertViewIs('dashboard');
    }

    public function test_dashboard_renders_when_there_are_no_logs(): void
    {
        $this->get('/')
            ->assertOk()
            ->assertViewIs('dashboard')
            ->assertViewHas('latest', null)
            ->assertViewHas('totalRecords', 0);
    }
}
