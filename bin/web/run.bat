chcp 65001
@echo off
echo ========================================
echo    Запуск локального сервера для WebGL
echo ========================================
echo.

cd /d "%~dp0"

echo Запуск Python сервера...
echo.

REM Запускаем сервер в ФОНОВОМ режиме (start /b)
start /b python -m http.server 8000

REM Ждем 2 секунды, пока сервер запустится
timeout /t 2 /nobreak >nul

REM Открываем браузер
echo Открываем браузер...
start http://localhost:8000

echo.
echo Сервер запущен. Для остановки закройте это окно и нажмите Ctrl+C
echo.

REM Держим окно открытым, чтобы можно было остановить сервер
pause >nul