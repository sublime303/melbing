<?php

namespace App\Http\Controllers;

use App\Models\ShipLog;

class DashboardController extends Controller
{
    public function index()
    {
        $latest = ShipLog::orderByDesc('unix_time')->first();

        $totalRecords = ShipLog::count();
        $firstRecord  = ShipLog::orderBy('unix_time')->first();

        return view('dashboard', compact('latest', 'totalRecords', 'firstRecord'));
    }
}
