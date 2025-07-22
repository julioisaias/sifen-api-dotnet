#!/bin/bash

# Script de pruebas para SIFEN API
# Reemplaza localhost:5063 con la URL real del API cuando esté funcionando

API_BASE_URL="http://localhost:5063/api/v1"
API_KEY="your-api-key-here"

echo "=========================================="
echo "PRUEBAS COMPLETAS DE SIFEN API"
echo "=========================================="

# Headers comunes
HEADERS=(-H "Content-Type: application/json" -H "X-API-Key: $API_KEY" -H "Accept: application/json")

echo ""
echo "1. FACTURAS ELECTRÓNICAS"
echo "----------------------------------------"

# 1.1 Crear una factura
echo "1.1 Creando nueva factura..."
FACTURA_RESPONSE=$(curl -s -X POST "$API_BASE_URL/facturas" "${HEADERS[@]}" \
  -d '{
    "rucEmisor": "80069590-1",
    "razonSocial": "EMPRESA DEMO S.A.",
    "cliente": {
      "ruc": "80056234-5",
      "razonSocial": "CLIENTE DEMO S.R.L.",
      "direccion": "Asunción, Paraguay"
    },
    "items": [
      {
        "descripcion": "Producto de prueba 1",
        "cantidad": 2,
        "precioUnitario": 75000,
        "descuento": 5000
      },
      {
        "descripcion": "Producto de prueba 2",
        "cantidad": 1,
        "precioUnitario": 50000,
        "descuento": 0
      }
    ],
    "total": 195000
  }')
echo "Respuesta: $FACTURA_RESPONSE"

# 1.2 Obtener factura por ID (usando un ID de ejemplo)
echo ""
echo "1.2 Obteniendo factura por ID..."
FACTURA_ID="123e4567-e89b-12d3-a456-426614174000"
curl -s -X GET "$API_BASE_URL/facturas/$FACTURA_ID" "${HEADERS[@]}" | jq '.'

# 1.3 Listar facturas con paginación
echo ""
echo "1.3 Listando facturas (página 1, 5 elementos)..."
curl -s -X GET "$API_BASE_URL/facturas?page=1&size=5" "${HEADERS[@]}" | jq '.'

# 1.4 Enviar factura a SIFEN
echo ""
echo "1.4 Enviando factura a SIFEN..."
curl -s -X POST "$API_BASE_URL/facturas/$FACTURA_ID/enviar-sifen" "${HEADERS[@]}" | jq '.'

echo ""
echo "2. NOTAS DE CRÉDITO"
echo "----------------------------------------"

# 2.1 Crear nota de crédito
echo "2.1 Creando nota de crédito..."
curl -s -X POST "$API_BASE_URL/notas-credito" "${HEADERS[@]}" \
  -d '{
    "documentoAsociadoCdc": "01800695901001001000000123456789012345",
    "motivo": "Devolución de mercadería defectuosa",
    "total": 50000,
    "items": [
      {
        "descripcion": "Devolución producto defectuoso",
        "cantidad": 1,
        "precioUnitario": 50000,
        "descuento": 0
      }
    ]
  }' | jq '.'

# 2.2 Obtener nota de crédito
echo ""
echo "2.2 Obteniendo nota de crédito..."
NOTA_CREDITO_ID="223e4567-e89b-12d3-a456-426614174001"
curl -s -X GET "$API_BASE_URL/notas-credito/$NOTA_CREDITO_ID" "${HEADERS[@]}" | jq '.'

echo ""
echo "3. NOTAS DE DÉBITO"
echo "----------------------------------------"

# 3.1 Crear nota de débito
echo "3.1 Creando nota de débito..."
curl -s -X POST "$API_BASE_URL/notas-debito" "${HEADERS[@]}" \
  -d '{
    "documentoAsociadoCdc": "01800695901001001000000123456789012345",
    "motivo": "Intereses por mora",
    "total": 25000,
    "items": [
      {
        "descripcion": "Intereses por pago tardío",
        "cantidad": 1,
        "precioUnitario": 25000,
        "descuento": 0
      }
    ]
  }' | jq '.'

# 3.2 Obtener nota de débito
echo ""
echo "3.2 Obteniendo nota de débito..."
NOTA_DEBITO_ID="323e4567-e89b-12d3-a456-426614174002"
curl -s -X GET "$API_BASE_URL/notas-debito/$NOTA_DEBITO_ID" "${HEADERS[@]}" | jq '.'

echo ""
echo "4. EVENTOS"
echo "----------------------------------------"

# 4.1 Cancelar documento
echo "4.1 Cancelando documento..."
curl -s -X POST "$API_BASE_URL/eventos/cancelar" "${HEADERS[@]}" \
  -d '{
    "documentoId": "'$FACTURA_ID'",
    "motivo": "Error en la facturación, datos incorrectos"
  }' | jq '.'

# 4.2 Inutilizar rango de numeración
echo ""
echo "4.2 Inutilizando rango de numeración..."
curl -s -X POST "$API_BASE_URL/eventos/inutilizar" "${HEADERS[@]}" \
  -d '{
    "rangoInicio": 100,
    "rangoFin": 110,
    "motivo": "Timbrado vencido, cambio de serie"
  }' | jq '.'

# 4.3 Registrar conformidad
echo ""
echo "4.3 Registrando conformidad..."
curl -s -X POST "$API_BASE_URL/eventos/conformidad" "${HEADERS[@]}" \
  -d '{
    "documentoId": "'$FACTURA_ID'"
  }' | jq '.'

echo ""
echo "5. CONSULTAS"
echo "----------------------------------------"

# 5.1 Consultar documento por CDC
echo "5.1 Consultando documento por CDC..."
CDC="01800695901001001000000123456789012345"
curl -s -X GET "$API_BASE_URL/consultas/documento/$CDC" "${HEADERS[@]}" | jq '.'

# 5.2 Consultar documentos por RUC
echo ""
echo "5.2 Consultando documentos por RUC..."
RUC="80069590-1"
curl -s -X GET "$API_BASE_URL/consultas/ruc/$RUC?fechaInicio=2025-01-01&fechaFin=2025-12-31" "${HEADERS[@]}" | jq '.'

# 5.3 Consultar lote
echo ""
echo "5.3 Consultando lote..."
LOTE_ID="423e4567-e89b-12d3-a456-426614174003"
curl -s -X GET "$API_BASE_URL/consultas/lote/$LOTE_ID" "${HEADERS[@]}" | jq '.'

echo ""
echo "6. PRUEBAS DE ERROR"
echo "----------------------------------------"

# 6.1 Probar endpoint inexistente
echo "6.1 Probando endpoint inexistente (debe devolver 404)..."
curl -s -X GET "$API_BASE_URL/endpoint-inexistente" "${HEADERS[@]}" -w "HTTP Status: %{http_code}\n"

# 6.2 Probar sin API Key
echo ""
echo "6.2 Probando sin API Key (debe devolver 401)..."
curl -s -X GET "$API_BASE_URL/facturas" -H "Content-Type: application/json" -w "HTTP Status: %{http_code}\n"

# 6.3 Probar con datos inválidos
echo ""
echo "6.3 Probando con datos inválidos (debe devolver 400)..."
curl -s -X POST "$API_BASE_URL/facturas" "${HEADERS[@]}" \
  -d '{
    "rucEmisor": "",
    "total": "invalid"
  }' -w "HTTP Status: %{http_code}\n"

echo ""
echo "=========================================="
echo "PRUEBAS COMPLETADAS"
echo "=========================================="