chcp 65001
@echo off
cd /d "%~dp0"
echo 📦 Установка http-server...
call npm install -g http-server

echo.
echo 🚀 Запуск сервера с поддержкой Gzip...
echo 📁 Папка: %cd%
echo.

REM http-server автоматически обрабатывает .gz файлы
start http://localhost:8080
http-server -p 8080 --gzip

pause