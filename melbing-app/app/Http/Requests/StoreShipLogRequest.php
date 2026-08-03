<?php

namespace App\Http\Requests;

use Illuminate\Contracts\Validation\ValidationRule;
use Illuminate\Foundation\Http\FormRequest;

class StoreShipLogRequest extends FormRequest
{
    /**
     * Determine if the user is authorized to make this request.
     */
    public function authorize(): bool
    {
        return true;
    }

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array<string, ValidationRule|array<mixed>|string>
     */
    public function rules(): array
    {
        return [
            'unix_time' => 'required|integer',
            'humidity' => 'nullable|numeric',
            'inside_temp' => 'nullable|numeric',
            'outside_temp' => 'nullable|numeric',
            'water_temp' => 'nullable|numeric',
            'refrigerator_temp' => 'nullable|numeric',
            'pressure' => 'nullable|numeric',
            'consumer_bat_v' => 'nullable|numeric',
            'start_bat_v' => 'nullable|numeric',
            'shore_power' => 'nullable|boolean',
            'wind_speed' => 'nullable|numeric',
            'relative_wind_angle' => 'nullable|numeric',
            'true_wind_angle' => 'nullable|numeric',
            'heating_element' => 'nullable|boolean',
            'dehumidifier' => 'nullable|boolean',
            'battery_charger' => 'nullable|boolean',
            'solar_charger' => 'nullable|boolean',
            'daylight_saving' => 'nullable|boolean',
            'timezone' => 'nullable|string|max:50',
        ];
    }

    /**
     * Get custom messages for validator errors.
     *
     * @return array<string, string>
     */
    public function messages(): array
    {
        return [
            'unix_time.required' => 'A Unix timestamp is required.',
            'unix_time.integer' => 'The Unix timestamp must be an integer.',
            'timezone.max' => 'The timezone may not be greater than 50 characters.',
        ];
    }
}
