@echo off
setlocal enabledelayedexpansion

REM Set paths relative to batch script directory (current folder)
set "PROJECT_DIR=%~dp0"

REM Check for Agent Mode argument
if "%~1"=="--agent" (
    echo [Agent Mode] Launching application with low verbosity...
    dotnet run --project "%PROJECT_DIR%." --arch x64 --verbosity quiet
) else if "%~1"=="/agent" (
    echo [Agent Mode] Launching application with low verbosity...
    dotnet run --project "%PROJECT_DIR%." --arch x64 --verbosity quiet
) else (
    echo [User Mode] Launching application under x64 emulation...
    dotnet run --project "%PROJECT_DIR%." --arch x64
)
