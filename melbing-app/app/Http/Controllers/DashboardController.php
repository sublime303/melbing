<?php

namespace App\Http\Controllers;

use App\Models\ShipLog;
use Illuminate\View\View;

class DashboardController extends Controller
{
    public function index(): View
    {
        $latest = ShipLog::query()->orderByDesc('unix_time')->first();

        $totalRecords = ShipLog::query()->count();
        $firstRecord = ShipLog::query()->orderBy('unix_time')->first();

        return view('dashboard', compact('latest', 'totalRecords', 'firstRecord'));
    }
}
