# SIFEN - Flujo de Creación de Factura Electrónica

## Flujo Secuencial Completo

Este diagrama muestra el flujo completo de creación de una factura electrónica desde la solicitud del cliente hasta la respuesta final.

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'actorBkg': '#e3f2fd',
    'actorBorder': '#1976d2',
    'actorTextColor': '#0d47a1',
    'activationBkgColor': '#bbdefb',
    'activationBorderColor': '#1976d2',
    'sequenceNumberColor': '#ffffff',
    'sectionBkgColor': '#e8f5e8',
    'altBkgColor': '#fff3e0'
  }
}}%%
sequenceDiagram
    participant C as 👤 Cliente
    participant API as 🎯 API Controller
    participant H as ⚙️ Handler
    participant V as ✅ Validator
    participant R as 📚 Repository
    participant X as 📄 XML Generator
    participant S as ✍️ Signer
    participant Q as 📱 QR Generator
    participant K as 📋 KUDE Generator
    participant SF as 🏛️ SIFEN Client
    participant SET as 🏛️ SIFEN (SET)
    participant DB as 🗄️ Database
    
    C->>+API: POST /api/v1/facturas
    Note over C,API: 📄 Solicitud de nueva factura
    
    API->>+V: Validar FacturaDto
    V-->>-API: Resultado de validación
    
    alt ❌ Validación fallida
        API-->>C: 400 Bad Request
        Note over API,C: ⚠️ Datos inválidos
    else ✅ Validación exitosa
        API->>+H: CreateFacturaCommand
        
        rect rgb(240, 248, 255)
        Note over H,DB: 📋 Obtención de datos base
        H->>+R: GetContribuyente
        R->>+DB: Consultar contribuyente
        DB-->>-R: Datos del contribuyente
        R-->>-H: Información completa
        
        H->>+R: GetTimbradoVigente
        R->>+DB: Consultar timbrado activo
        DB-->>-R: Timbrado válido
        R-->>-H: Datos de timbrado
        end
        
        rect rgb(240, 255, 240)
        Note over H,DB: 🏗️ Creación del documento
        H->>H: Crear DocumentoElectronico
        H->>H: Generar CDC único
        H->>+R: Guardar documento
        R->>+DB: Insertar registro
        DB-->>-R: Confirmación
        R-->>-H: Documento guardado
        end
        
        rect rgb(255, 248, 240)
        Note over H,K: 📝 Generación de archivos
        H->>+X: Generar XML
        X-->>-H: Contenido XML
        
        H->>+S: Firmar XML
        S-->>-H: XML firmado
        
        H->>+Q: Generar código QR
        Q-->>-H: Datos del QR
        
        H->>+K: Generar PDF KUDE
        K-->>-H: Bytes del PDF
        end
        
        alt 🌐 Emisión normal
            rect rgb(248, 255, 248)
            Note over H,SET: 🚀 Envío a SIFEN
            H->>+SF: Enviar a SIFEN
            SF->>+SET: Petición SOAP
            SET-->>-SF: Respuesta SOAP
            SF-->>-H: Resultado
            end
            
            alt ✅ SIFEN aprobado
                H->>H: Actualizar estado: Aprobado
                H->>+R: Guardar cambios
                R->>+DB: Actualizar registro
                DB-->>-R: Confirmación
                R-->>-H: Estado actualizado
                H->>API: Respuesta exitosa
                API-->>C: 201 Created + Datos
                Note over API,C: ✨ Factura creada exitosamente
            else ❌ SIFEN rechazado
                H->>H: Actualizar estado: Error
                H->>+R: Guardar error
                R->>+DB: Actualizar con error
                DB-->>-R: Confirmación
                R-->>-H: Error registrado
                H->>API: Respuesta de error
                API-->>C: 400 Bad Request
                Note over API,C: ⚠️ Factura rechazada por SIFEN
            end
        else ⚡ Contingencia
            rect rgb(255, 245, 245)
            Note over H,DB: 📦 Modo contingencia activo
            H->>+R: Guardar para procesar después
            R->>+DB: Actualizar estado contingencia
            DB-->>-R: Guardado en cola
            R-->>-H: Pendiente de envío
            H->>API: Respuesta exitosa
            API-->>C: 201 Created + Datos
            Note over API,C: ⏳ Factura en contingencia
            end
        
        H-->>-API: Finalizar handler
        API-->>-C: Respuesta final
        end
    end
```

## Descripción del Flujo

### 📥 Fase de Recepción
1. **Solicitud inicial**: El cliente envía una petición POST con los datos de la factura
2. **Validación**: Se validan los datos de entrada usando validators específicos
3. **Manejo de errores**: Si la validación falla, se retorna error 400

### 📋 Fase de Obtención de Datos Base
1. **Datos del contribuyente**: Se obtienen los datos completos del emisor
2. **Timbrado vigente**: Se verifica y obtiene el timbrado activo para la facturación
3. **Validaciones de negocio**: Se verifican las reglas de negocio específicas

### 🏗️ Fase de Creación del Documento
1. **Instanciación**: Se crea la entidad DocumentoElectronico
2. **CDC único**: Se genera el Código de Control único
3. **Persistencia**: Se guarda el documento en la base de datos

### 📝 Fase de Generación de Archivos
1. **XML SIFEN**: Se genera el XML con el formato requerido por SIFEN
2. **Firmado digital**: Se aplica la firma digital al XML
3. **Código QR**: Se genera el código QR con la información del documento
4. **PDF KUDE**: Se genera el PDF con el formato KUDE requerido

### 🚀 Fase de Envío a SIFEN
#### 🌐 Emisión Normal
- Se envía el documento a SIFEN mediante SOAP
- Si es aprobado: se actualiza el estado y se responde exitosamente
- Si es rechazado: se registra el error y se informa al cliente

#### ⚡ Modo Contingencia
- El documento se guarda para procesamiento posterior
- Se responde exitosamente al cliente
- El documento se procesa cuando SIFEN esté disponible

## Códigos de Respuesta

- **201 Created**: Factura creada exitosamente
- **400 Bad Request**: Error en validación o rechazo de SIFEN
- **500 Internal Server Error**: Error interno del sistema