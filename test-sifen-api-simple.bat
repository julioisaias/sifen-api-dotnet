@echo off
REM Script simplificado de pruebas para SIFEN API

set API_BASE_URL=http://localhost:5001/api/v1
set API_KEY=test-api-key-123

echo ==========================================
echo PRUEBA DE FACTURA ELECTRONICA
echo ==========================================

echo.
echo Creando factura electronica...
curl -X POST "%API_BASE_URL%/facturas" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"rucEmisor\":\"80069590-1\",\"razonSocial\":\"EMPRESA DEMO S.A.\",\"cliente\":{\"ruc\":\"80056234-5\",\"razonSocial\":\"CLIENTE DEMO S.R.L.\",\"direccion\":\"Asunción, Paraguay\"},\"items\":[{\"descripcion\":\"Producto de prueba 1\",\"cantidad\":2,\"precioUnitario\":75000,\"descuento\":5000},{\"descripcion\":\"Producto de prueba 2\",\"cantidad\":1,\"precioUnitario\":50000,\"descuento\":0}],\"total\":195000}" ^
  -w "\n\nHTTP Status: %%{http_code}\n"

echo.
echo ==========================================
echo Si aparece error 500, ejecutar:
echo 1. dotnet ef database update --project src/SifenApi.Infrastructure --startup-project src/SifenApi.WebApi
echo 2. sqlcmd -S (localdb)\mssqllocaldb -d SifenApiDb_Dev -i scripts\init-test-db.sql
echo ==========================================

pause