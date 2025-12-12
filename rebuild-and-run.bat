@echo off
echo =======================================
echo Employee Management System - REBUILD
echo =======================================
echo.
echo This will close any running instances
echo and rebuild with the latest fixes.
echo.
pause

cd EmployeeCRUD

echo Killing any running instances...
taskkill /F /IM EmployeeCRUD.exe 2>nul
timeout /t 2 /nobreak >nul

echo.
echo Cleaning build...
dotnet clean

echo.
echo Restoring packages...
dotnet restore

echo.
echo Building application...
dotnet build

if %ERRORLEVEL% EQU 0 (
    echo.
    echo =======================================
    echo BUILD SUCCESSFUL!
    echo =======================================
    echo.
    echo Starting application...
    echo GUI window will open shortly...
    echo.
    dotnet run
) else (
    echo.
    echo =======================================
    echo BUILD FAILED!
    echo =======================================
    echo Please check the errors above.
    echo.
    pause
)
