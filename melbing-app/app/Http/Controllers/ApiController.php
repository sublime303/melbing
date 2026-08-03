<?php

namespace App\Http\Controllers;

use App\Http\Requests\ListShipLogsRequest;
use App\Http\Requests\StoreShipLogRequest;
use App\Http\Resources\ShipLogResource;
use App\Models\ShipLog;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Resources\Json\AnonymousResourceCollection;

class ApiController extends Controller
{
    public function store(StoreShipLogRequest $request): JsonResponse
    {
        ShipLog::query()->create($request->validated());

        return response()->json(['status' => 'ok'], 201);
    }

    public function index(ListShipLogsRequest $request): AnonymousResourceCollection
    {
        $since = now()->subHours($request->hours())->timestamp;

        $logs = ShipLog::query()
            ->where('unix_time', '>=', $since)
            ->orderBy('unix_time')
            ->get();

        return ShipLogResource::collection($logs);
    }
}
