@echo off
cd /d "%~dp0"
start "" python dashboard.py
timeout /t 2 >nul
start "" http://127.0.0.1:8080
