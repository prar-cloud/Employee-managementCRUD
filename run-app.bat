@echo off
echo ===================================
echo Employee Management System
echo ===================================
echo.
echo Mode: In-Memory (Data not saved)
echo To use MongoDB: See START_HERE.md
echo.

cd EmployeeCRUD

echo Restoring packages...
dotnet restore

echo.
echo Building application...
dotnet build

echo.
echo Starting application...
echo GUI window will open shortly...
dotnet run

pause
