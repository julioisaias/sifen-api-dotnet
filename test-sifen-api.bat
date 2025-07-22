@echo off
REM Script de pruebas para SIFEN API en Windows
REM Reemplaza localhost:5063 con la URL real del API cuando esté funcionando

set API_BASE_URL=http://localhost:5001/api/v1
set API_KEY=test-api-key-123

echo ==========================================
echo PRUEBAS COMPLETAS DE SIFEN API
echo ==========================================

echo.
echo 1. FACTURAS ELECTRONICAS
echo ----------------------------------------

REM 1.1 Crear una factura
echo 1.1 Creando nueva factura...
curl -X POST "%API_BASE_URL%/facturas" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"rucEmisor\":\"80069590-1\",\"razonSocial\":\"EMPRESA DEMO S.A.\",\"cliente\":{\"ruc\":\"80056234-5\",\"razonSocial\":\"CLIENTE DEMO S.R.L.\",\"direccion\":\"Asunción, Paraguay\"},\"items\":[{\"descripcion\":\"Producto de prueba 1\",\"cantidad\":2,\"precioUnitario\":75000,\"descuento\":5000},{\"descripcion\":\"Producto de prueba 2\",\"cantidad\":1,\"precioUnitario\":50000,\"descuento\":0}],\"total\":195000}"

echo.
echo 1.2 Obteniendo factura por ID...
set FACTURA_ID=123e4567-e89b-12d3-a456-426614174000
curl -X GET "%API_BASE_URL%/facturas/%FACTURA_ID%" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 1.3 Listando facturas (página 1, 5 elementos)...
curl -X GET "%API_BASE_URL%/facturas?page=1&size=5" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 1.4 Enviando factura a SIFEN...
curl -X POST "%API_BASE_URL%/facturas/%FACTURA_ID%/enviar-sifen" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 2. NOTAS DE CREDITO
echo ----------------------------------------

echo 2.1 Creando nota de crédito...
curl -X POST "%API_BASE_URL%/notas-credito" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"documentoAsociadoCdc\":\"01800695901001001000000123456789012345\",\"motivo\":\"Devolución de mercadería defectuosa\",\"total\":50000,\"items\":[{\"descripcion\":\"Devolución producto defectuoso\",\"cantidad\":1,\"precioUnitario\":50000,\"descuento\":0}]}"

echo.
echo 2.2 Obteniendo nota de crédito...
set NOTA_CREDITO_ID=223e4567-e89b-12d3-a456-426614174001
curl -X GET "%API_BASE_URL%/notas-credito/%NOTA_CREDITO_ID%" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 3. NOTAS DE DEBITO
echo ----------------------------------------

echo 3.1 Creando nota de débito...
curl -X POST "%API_BASE_URL%/notas-debito" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"documentoAsociadoCdc\":\"01800695901001001000000123456789012345\",\"motivo\":\"Intereses por mora\",\"total\":25000,\"items\":[{\"descripcion\":\"Intereses por pago tardío\",\"cantidad\":1,\"precioUnitario\":25000,\"descuento\":0}]}"

echo.
echo 3.2 Obteniendo nota de débito...
set NOTA_DEBITO_ID=323e4567-e89b-12d3-a456-426614174002
curl -X GET "%API_BASE_URL%/notas-debito/%NOTA_DEBITO_ID%" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 4. EVENTOS
echo ----------------------------------------

echo 4.1 Cancelando documento...
curl -X POST "%API_BASE_URL%/eventos/cancelar" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"documentoId\":\"%FACTURA_ID%\",\"motivo\":\"Error en la facturación, datos incorrectos\"}"

echo.
echo 4.2 Inutilizando rango de numeración...
curl -X POST "%API_BASE_URL%/eventos/inutilizar" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"rangoInicio\":100,\"rangoFin\":110,\"motivo\":\"Timbrado vencido, cambio de serie\"}"

echo.
echo 4.3 Registrando conformidad...
curl -X POST "%API_BASE_URL%/eventos/conformidad" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"documentoId\":\"%FACTURA_ID%\"}"

echo.
echo 5. CONSULTAS
echo ----------------------------------------

echo 5.1 Consultando documento por CDC...
set CDC=01800695901001001000000123456789012345
curl -X GET "%API_BASE_URL%/consultas/documento/%CDC%" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 5.2 Consultando documentos por RUC...
set RUC=80069590-1
curl -X GET "%API_BASE_URL%/consultas/ruc/%RUC%?fechaInicio=2025-01-01&fechaFin=2025-12-31" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 5.3 Consultando lote...
set LOTE_ID=423e4567-e89b-12d3-a456-426614174003
curl -X GET "%API_BASE_URL%/consultas/lote/%LOTE_ID%" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json"

echo.
echo 6. PRUEBAS DE ERROR
echo ----------------------------------------

echo 6.1 Probando endpoint inexistente (debe devolver 404)...
curl -X GET "%API_BASE_URL%/endpoint-inexistente" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -w "HTTP Status: %%{http_code}"

echo.
echo 6.2 Probando sin API Key (debe devolver 401)...
curl -X GET "%API_BASE_URL%/facturas" ^
  -H "Content-Type: application/json" ^
  -w "HTTP Status: %%{http_code}"

echo.
echo 6.3 Probando con datos inválidos (debe devolver 400)...
curl -X POST "%API_BASE_URL%/facturas" ^
  -H "Content-Type: application/json" ^
  -H "X-API-Key: %API_KEY%" ^
  -H "Accept: application/json" ^
  -d "{\"rucEmisor\":\"\",\"total\":\"invalid\"}" ^
  -w "HTTP Status: %%{http_code}"

echo.
echo ==========================================
echo PRUEBAS COMPLETADAS
echo ==========================================

pause