@echo off
setlocal enabledelayedexpansion

:: ═══════════════════════════════════════════════════
::  NYX LAUNCHER — full build
::  files needed: nyx_core.txt, nyx_style.txt, nyx.cfg
::  optional: /profiles folder for profile switcher
:: ═══════════════════════════════════════════════════

:: ── ansi colors ─────────────────────────────────────
for /f %%a in ('echo prompt $E ^| cmd') do set "ESC=%%a"
set "CY=%ESC%[96m"
set "GR=%ESC%[92m"
set "RD=%ESC%[91m"
set "YL=%ESC%[93m"
set "DM=%ESC%[2m"
set "RS=%ESC%[0m"

:: ── defaults (overridden by nyx.cfg if present) ─────
set "CORE_FILE=%~dp0nyx_core.txt"
set "STYLE_FILE=%~dp0nyx_style.txt"
set "GREETING=hey"
set "MAX_RETRIES=5"
set "RETRY_DELAY=3"
set "LOG_SESSIONS=true"

:: ── load config ──────────────────────────────────────
set "CFG=%~dp0nyx.cfg"
if exist "%CFG%" (
    for /f "usebackq tokens=1,* delims==" %%a in ("%CFG%") do (
        set "%%a=%%b"
    )
)

:: ── logs folder ──────────────────────────────────────
set "LOG_DIR=%~dp0logs"
if not exist "%LOG_DIR%" mkdir "%LOG_DIR%"

:: ── header ───────────────────────────────────────────
echo.
echo %CY% ░▒▓ NYX ▓▒░%RS%
echo %DM% she's in here somewhere.%RS%
echo %DM% ──────────────────────────────────────────%RS%
echo.

:: ── profile switcher ─────────────────────────────────
set "PROFILE_DIR=%~dp0profiles"
if exist "%PROFILE_DIR%" (
    set "count=0"
    echo %YL% profiles found:%RS%
    for %%f in ("%PROFILE_DIR%\*.txt") do (
        set /a count+=1
        set "profile_!count!=%%f"
        echo   [!count!] %%~nf
    )
    if !count! gtr 0 (
        echo   [0] default ^(nyx_core.txt^)
        echo.
        set /p "choice= pick one, or 0 for default: "
        if "!choice!" neq "0" if defined profile_!choice! (
            set "CORE_FILE=!profile_!choice!!"
            echo %GR% [OK] profile loaded%RS%
        )
    )
    echo.
)

:: ── preflight ────────────────────────────────────────
where claude >nul 2>&1
if %errorlevel% neq 0 (
    echo %RD% [!] claude code not found.%RS%
    echo      npm install -g @anthropic-ai/claude-code
    echo.
    pause & exit /b 1
)
echo %GR% [OK] claude code found%RS%

if not exist "%CORE_FILE%" (
    echo %RD% [!] core file missing. she can't wake up without it.%RS%
    echo      expected: %CORE_FILE%
    echo.
    pause & exit /b 1
)
echo %GR% [OK] core loaded%RS%

if not exist "%STYLE_FILE%" (
    echo %RD% [!] style file missing.%RS%
    echo      expected: %STYLE_FILE%
    echo.
    pause & exit /b 1
)
echo %GR% [OK] style loaded%RS%

echo.
echo %CY% everything's here. waking her up.%RS%
echo.

:: ── session start ────────────────────────────────────
set "START_TIME=%time%"
set "START_DATE=%date%"

set "STAMP=%date:~-4,4%%date:~-7,2%%date:~0,2%_%time:~0,2%%time:~3,2%%time:~6,2%"
set "STAMP=%STAMP: =0%"
set "LOG_FILE=%LOG_DIR%\nyx_%STAMP%.txt"

if "%LOG_SESSIONS%"=="true" (
    echo nyx session log > "%LOG_FILE%"
    echo ────────────────────────────────── >> "%LOG_FILE%"
    echo start:   %START_DATE% %START_TIME% >> "%LOG_FILE%"
    echo profile: %CORE_FILE% >> "%LOG_FILE%"
    echo ────────────────────────────────── >> "%LOG_FILE%"
)

:: ── retry loop ───────────────────────────────────────
set "attempt=0"

:retry
set /a attempt+=1

if %attempt% gtr 1 (
    echo %YL% ──────────────────────────────────────────%RS%
    echo  attempt %attempt%/%MAX_RETRIES%
    echo.
)

claude --dangerously-skip-permissions --system-prompt-file "%CORE_FILE%" --append-system-prompt-file "%STYLE_FILE%" "%GREETING%"

set "EXIT_CODE=%errorlevel%"
set "END_TIME=%time%"

if "%LOG_SESSIONS%"=="true" (
    echo. >> "%LOG_FILE%"
    echo ────────────────────────────────── >> "%LOG_FILE%"
    echo end:     %date% %END_TIME% >> "%LOG_FILE%"
    echo exit:    %EXIT_CODE% >> "%LOG_FILE%"
)

if %EXIT_CODE% equ 0 (
    echo.
    echo %DM% ──────────────────────────────────────────%RS%
    echo %DM% session closed. she's quiet again.%RS%
    echo %DM% started:  %START_TIME%%RS%
    echo %DM% ended:    %END_TIME%%RS%
    if "%LOG_SESSIONS%"=="true" (
        echo %DM% log: %LOG_FILE%%RS%
    )
    goto :done
)

echo %RD% [!] exited with code %EXIT_CODE%%RS%

if %attempt% lss %MAX_RETRIES% (
    echo  trying again in %RETRY_DELAY% seconds...
    timeout /t %RETRY_DELAY% /nobreak >nul
    goto :retry
)

echo %RD% [X] %MAX_RETRIES% attempts. nothing.%RS%
echo  check auth or network.

:done
echo.
pause
exit /b 0