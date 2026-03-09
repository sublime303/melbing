<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('ship_logs', function (Blueprint $table) {
            $table->id();
            $table->bigInteger('unix_time');
            $table->float('humidity')->nullable();
            $table->float('inside_temp')->nullable();
            $table->float('outside_temp')->nullable();
            $table->float('water_temp')->nullable();
            $table->float('refrigerator_temp')->nullable();
            $table->float('pressure')->nullable();
            $table->float('consumer_bat_v')->nullable();
            $table->float('start_bat_v')->nullable();
            $table->boolean('shore_power')->default(false);
            $table->float('wind_speed')->nullable();
            $table->float('relative_wind_angle')->nullable();
            $table->float('true_wind_angle')->nullable();
            $table->boolean('heating_element')->default(false);
            $table->boolean('dehumidifier')->default(false);
            $table->boolean('battery_charger')->default(false);
            $table->boolean('solar_charger')->default(false);
            $table->boolean('daylight_saving')->default(false);
            $table->string('timezone')->nullable();
            $table->timestamps();

            $table->index('unix_time');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('ship_logs');
    }
};
