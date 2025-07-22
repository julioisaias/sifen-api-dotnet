@echo off
cd src\SifenApi.WebApi
set ASPNETCORE_ENVIRONMENT=Test
echo Iniciando SIFEN API en modo TEST...
dotnet run --urls http://localhost:5001