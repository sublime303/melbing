<?php

use App\Http\Controllers\ApiController;
use Illuminate\Support\Facades\Route;

Route::post('/log', [ApiController::class, 'store']);
Route::get('/logs', [ApiController::class, 'index']);
