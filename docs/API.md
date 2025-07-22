# SifenApi - API Documentation

## Overview

SifenApi es una API para la generación y gestión de documentos electrónicos según las especificaciones del SIFEN (Sistema de Facturación Electrónica Nacional) de Paraguay.

## Endpoints

### Documentos Electrónicos

#### Facturas
- `POST /api/v1/facturas` - Crear nueva factura
- `GET /api/v1/facturas/{id}` - Obtener factura por ID
- `GET /api/v1/facturas` - Listar facturas

#### Notas de Crédito
- `POST /api/v1/notas-credito` - Crear nueva nota de crédito
- `GET /api/v1/notas-credito/{id}` - Obtener nota de crédito por ID

#### Notas de Débito
- `POST /api/v1/notas-debito` - Crear nueva nota de débito
- `GET /api/v1/notas-debito/{id}` - Obtener nota de débito por ID

#### Notas de Remisión
- `POST /api/v1/notas-remision` - Crear nueva nota de remisión
- `GET /api/v1/notas-remision/{id}` - Obtener nota de remisión por ID

### Eventos
- `POST /api/v1/eventos/cancelar` - Cancelar documento
- `POST /api/v1/eventos/inutilizar` - Inutilizar rango de numeración

### Consultas
- `GET /api/v1/consultas/documento/{cdc}` - Consultar documento por CDC
- `GET /api/v1/consultas/ruc/{ruc}` - Consultar documentos por RUC

## Autenticación

La API utiliza autenticación por API Key. Incluir el header:
```
X-API-Key: your-api-key
```

## Modelos de Datos

### Factura
```json
{
  "rucEmisor": "string",
  "razonSocial": "string",
  "cliente": {
    "ruc": "string",
    "razonSocial": "string",
    "direccion": "string"
  },
  "items": [
    {
      "descripcion": "string",
      "cantidad": 0,
      "precioUnitario": 0,
      "descuento": 0
    }
  ]
}
```

## Códigos de Estado

- 200: OK
- 201: Created
- 400: Bad Request
- 401: Unauthorized
- 404: Not Found
- 500: Internal Server Error