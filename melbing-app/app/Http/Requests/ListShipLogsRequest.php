<?php

namespace App\Http\Requests;

use Illuminate\Contracts\Validation\ValidationRule;
use Illuminate\Foundation\Http\FormRequest;

class ListShipLogsRequest extends FormRequest
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
            'hours' => 'sometimes|integer|min:1|max:720',
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
            'hours.integer' => 'The hours parameter must be an integer.',
            'hours.min' => 'The hours parameter must be at least 1.',
            'hours.max' => 'The hours parameter may not be greater than 720.',
        ];
    }

    /**
     * Hours of history to return (default 24).
     */
    public function hours(): int
    {
        return (int) $this->validated('hours', 24);
    }
}
