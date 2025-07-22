# SIFEN - Modelo de Base de Datos

## Diagrama Entidad-Relación

Este diagrama muestra el modelo de base de datos completo con todas las entidades y sus relaciones.

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#e8f4f8',
    'primaryTextColor': '#0277bd',
    'primaryBorderColor': '#0288d1',
    'lineColor': '#01579b',
    'tertiaryColor': '#fff3e0',
    'background': '#ffffff'
  }
}}%%
erDiagram
    Contribuyente {
        guid Id PK "Identificador único"
        string Ruc UK "RUC del contribuyente"
        string RazonSocial "Nombre comercial"
        string NombreFantasia "Nombre de fantasía"
        int TipoContribuyente "Tipo de contribuyente"
        int TipoRegimen "Régimen tributario"
        bool Activo "Estado activo/inactivo"
        datetime FechaCreacion "Fecha de registro"
        datetime FechaModificacion "Ultima actualización"
    }
    
    Establecimiento {
        guid Id PK "Identificador único"
        string Codigo "Código de establecimiento"
        string Denominacion "Nombre del local"
        string Direccion "Dirección física"
        int Departamento "Departamento"
        int Distrito "Distrito"
        int Ciudad "Ciudad"
        guid ContribuyenteId FK "Dueño del establecimiento"
        bool Activo "Estado del establecimiento"
    }
    
    Timbrado {
        guid Id PK "Identificador único"
        string Numero UK "Número de timbrado"
        date FechaInicio "Fecha de inicio de vigencia"
        date FechaFin "Fecha de vencimiento"
        int Estado "Estado del timbrado"
        guid ContribuyenteId FK "Propietario del timbrado"
        datetime FechaCreacion "Fecha de registro"
    }
    
    DocumentoElectronico {
        guid Id PK "Identificador único"
        string CDC UK "Código de Control (CDC)"
        int TipoDocumento "Tipo de documento fiscal"
        string Establecimiento "Código del establecimiento"
        string PuntoExpedicion "Punto de expedición"
        string NumeroDocumento "Número correlativo"
        datetime FechaEmision "Fecha y hora de emisión"
        int Estado "Estado del documento"
        decimal Total "Monto total del documento"
        string XMLFirmado "XML con firma digital"
        string CodigoQR "Datos del código QR"
        guid TimbradoId FK "Timbrado utilizado"
        guid ContribuyenteId FK "Emisor del documento"
        guid ClienteId FK "Receptor del documento"
    }
    
    Item {
        guid Id PK "Identificador único"
        string Codigo "Código del producto/servicio"
        string Descripcion "Descripción del item"
        decimal Cantidad "Cantidad vendida"
        decimal PrecioUnitario "Precio por unidad"
        decimal MontoIva "IVA aplicado"
        decimal MontoTotal "Total del item"
        int TipoIva "Tipo de IVA aplicado"
        guid DocumentoElectronicoId FK "Documento al que pertenece"
    }
    
    Cliente {
        guid Id PK "Identificador único"
        bool EsContribuyente "Indica si es contribuyente"
        string Ruc "RUC si es contribuyente"
        string RazonSocial "Razón social"
        string DocumentoNumero "Número de documento"
        string Nombre "Nombre completo"
        string Email "Correo electrónico"
        string Telefono "Número de teléfono"
        datetime FechaCreacion "Fecha de registro"
    }
    
    EventoDocumento {
        guid Id PK "Identificador único"
        int TipoEvento "Tipo de evento"
        string Motivo "Motivo del evento"
        datetime FechaEvento "Fecha del evento"
        int Estado "Estado resultante"
        string Observaciones "Notas adicionales"
        guid DocumentoElectronicoId FK "Documento relacionado"
    }
    
    ActividadEconomica {
        guid Id PK "Identificador único"
        string Codigo "Código de actividad"
        string Descripcion "Descripción de la actividad"
        guid ContribuyenteId FK "Contribuyente que realiza"
        bool Principal "Actividad principal"
    }
    
    %% Relaciones principales
    Contribuyente ||--o{ Establecimiento : "tiene"
    Contribuyente ||--o{ ActividadEconomica : "realiza"
    Contribuyente ||--o{ Timbrado : "posee"
    Contribuyente ||--o{ DocumentoElectronico : "emite"
    
    Timbrado ||--o{ DocumentoElectronico : "autoriza"
    
    DocumentoElectronico ||--o{ Item : "contiene"
    DocumentoElectronico ||--o| Cliente : "dirigido_a"
    DocumentoElectronico ||--o{ EventoDocumento : "tiene"
    
    Cliente ||--o{ DocumentoElectronico : "recibe"
    
```

## Descripción de las Entidades

### 🏛️ Contribuyente
**Descripción**: Entidad principal que representa a una empresa o persona física que emite documentos fiscales.

**Campos clave**:
- **Ruc**: Registro Único de Contribuyentes (único)
- **TipoContribuyente**: Clasifica el tipo de contribuyente según SIFEN
- **TipoRegimen**: Régimen tributario al que pertenece
- **Activo**: Indica si el contribuyente está activo

### 🏢 Establecimiento
**Descripción**: Locales físicos donde se realizan las operaciones comerciales.

**Campos clave**:
- **Codigo**: Código único del establecimiento
- **Direccion**: Ubicación física del establecimiento
- **Departamento, Distrito, Ciudad**: División geográfica
- **ContribuyenteId**: Relación con el contribuyente propietario

### 📄 Timbrado
**Descripción**: Autorización fiscal que permite la emisión de documentos electrónicos.

**Campos clave**:
- **Numero**: Número único del timbrado (otorgado por SET)
- **FechaInicio/FechaFin**: Período de vigencia
- **Estado**: Estado actual del timbrado (vigente, vencido, etc.)

### 📋 DocumentoElectronico
**Descripción**: Entidad central que representa cualquier documento fiscal electrónico.

**Campos clave**:
- **CDC**: Código de Control único generado para cada documento
- **TipoDocumento**: Tipo de documento fiscal (factura, nota de crédito, etc.)
- **Estado**: Estado actual del documento (pendiente, aprobado, rechazado)
- **XMLFirmado**: XML del documento con firma digital
- **CodigoQR**: Datos para generar el código QR

### 📦 Item
**Descripción**: Líneas de detalle de los documentos electrónicos (productos/servicios).

**Campos clave**:
- **Codigo**: Código del producto o servicio
- **Cantidad**: Cantidad vendida
- **PrecioUnitario**: Precio por unidad
- **TipoIva**: Tipo de IVA aplicado (exento, 10%, 5%)

### 👤 Cliente
**Descripción**: Receptores de los documentos electrónicos.

**Campos clave**:
- **EsContribuyente**: Indica si el cliente es también contribuyente
- **Ruc**: RUC si es contribuyente
- **DocumentoNumero**: Cédula de identidad u otro documento

### 📈 EventoDocumento
**Descripción**: Registro de eventos que ocurren durante el ciclo de vida de un documento.

**Campos clave**:
- **TipoEvento**: Tipo de evento (creación, envío, aprobación, error, etc.)
- **FechaEvento**: Momento en que ocurrió el evento
- **Estado**: Estado resultante después del evento

### 💼 ActividadEconomica
**Descripción**: Actividades económicas que puede realizar un contribuyente.

**Campos clave**:
- **Codigo**: Código oficial de la actividad económica
- **Principal**: Indica si es la actividad principal del contribuyente

## Relaciones Principales

### Contribuyente → Establecimiento (1:N)
Un contribuyente puede tener múltiples establecimientos.

### Contribuyente → Timbrado (1:N)
Un contribuyente puede tener múltiples timbrados (histórico y vigentes).

### Contribuyente → DocumentoElectronico (1:N)
Un contribuyente emite múltiples documentos electrónicos.

### Timbrado → DocumentoElectronico (1:N)
Un timbrado autoriza múltiples documentos electrónicos.

### DocumentoElectronico → Item (1:N)
Un documento contiene múltiples líneas de detalle.

### DocumentoElectronico → EventoDocumento (1:N)
Un documento tiene múltiples eventos a lo largo de su ciclo de vida.

### Cliente → DocumentoElectronico (1:N)
Un cliente puede recibir múltiples documentos electrónicos.

## Índices Recomendados

### Índices Únicos
- `Contribuyente.Ruc`
- `Timbrado.Numero`
- `DocumentoElectronico.CDC`

### Índices de Búsqueda
- `DocumentoElectronico.Estado`
- `DocumentoElectronico.FechaEmision`
- `DocumentoElectronico.ContribuyenteId`
- `EventoDocumento.DocumentoElectronicoId`
- `Item.DocumentoElectronicoId`

### Índices Compuestos
- `(ContribuyenteId, Estado)` en DocumentoElectronico
- `(TimbradoId, FechaEmision)` en DocumentoElectronico
- `(DocumentoElectronicoId, TipoEvento)` en EventoDocumento