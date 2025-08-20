@echo off
chcp 65001 > nul
echo Building Messenger...

if exist "Build" rmdir /s /q Build
mkdir Build

echo.
echo Building Server...
dotnet build Server/Server.csproj -c Release -o Build/Server

if %errorlevel% neq 0 (
    echo Server build failed!
    pause
    exit /b 1
)

echo.
echo Building Client...
dotnet build Client/Client.csproj -c Release -o Build/Client

if %errorlevel% neq 0 (
    echo Client build failed!
    pause
    exit /b 1
)

echo.
echo Build successful! Files are in Build/ folder
echo.
echo To run server: Build\Server\Server.exe
echo To run client: Build\Client\Client.exe
echo.
pause