# SIFEN - Arquitectura de Componentes

## Diagrama de Componentes del Sistema

Este diagrama muestra la arquitectura de componentes con las aplicaciones frontend, el núcleo de la API, servicios de negocio e infraestructura.

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#e8f4f8',
    'primaryTextColor': '#0277bd',
    'primaryBorderColor': '#0288d1',
    'lineColor': '#01579b',
    'secondaryColor': '#e8f5e8',
    'tertiaryColor': '#fff3e0',
    'quaternaryColor': '#fce4ec'
  }
}}%%
graph LR
    subgraph "📱 Aplicaciones Frontend"
        A1["🌐 Web SPA<br/><small>React/Angular</small>"]
        A2["📱 Mobile App<br/><small>React Native</small>"]
        A3["💻 Desktop App<br/><small>WPF/Electron</small>"]
    end
    
    subgraph "🚀 Núcleo SIFEN API"
        B1["🎯 REST API<br/><small>.NET 9</small>"]
        B2["⏰ Background Jobs<br/><small>Hangfire</small>"]
        B3["⚡ SignalR Hub<br/><small>Real-time</small>"]
    end
    
    subgraph "⚙️ Servicios de Negocio"
        C1["📄 XML Service<br/><small>Generación</small>"]
        C2["✍️ Signature Service<br/><small>Firmado digital</small>"]
        C3["📱 QR Service<br/><small>Códigos QR</small>"]
        C4["📋 PDF Service<br/><small>KUDE/Reportes</small>"]
        C5["📧 Email Service<br/><small>Notificaciones</small>"]
        C6["💾 Storage Service<br/><small>Archivos</small>"]
    end
    
    subgraph "🏗️ Infraestructura"
        D1["🗄️ SQL Server<br/><small>Base de datos</small>"]
        D2["⚡ Redis Cache<br/><small>Cache distribuido</small>"]
        D3["📁 Blob Storage<br/><small>Archivos</small>"]
        D4["📦 Message Queue<br/><small>Cola de mensajes</small>"]
    end
    
    subgraph "🌐 Servicios Externos"
        E1["🏛️ SIFEN SET<br/><small>Gobierno</small>"]
        E2["📧 SMTP Server<br/><small>Correo</small>"]
        E3["🗝 Certificate Authority<br/><small>Certificados</small>"]
    end
    
    A1 --> B1
    A2 --> B1
    A3 --> B1
    
    B1 --> C1
    B1 --> C2
    B1 --> C3
    B1 --> C4
    B1 --> C5
    B1 --> C6
    
    B2 --> D4
    B3 --> D2
    
    C1 --> D1
    C2 --> E3
    C5 --> E2
    C6 --> D3
    
    B1 --> E1
    B2 --> E1
    
    classDef frontendStyle fill:#e3f2fd,stroke:#1976d2,stroke-width:2px,color:#0d47a1
    classDef coreStyle fill:#e8f5e8,stroke:#388e3c,stroke-width:2px,color:#1b5e20
    classDef serviceStyle fill:#fff3e0,stroke:#f57c00,stroke-width:2px,color:#e65100
    classDef infraStyle fill:#fce4ec,stroke:#c2185b,stroke-width:2px,color:#880e4f
    classDef externalStyle fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px,color:#4a148c
    
    class A1,A2,A3 frontendStyle
    class B1,B2,B3 coreStyle
    class C1,C2,C3,C4,C5,C6 serviceStyle
    class D1,D2,D3,D4 infraStyle
    class E1,E2,E3 externalStyle
```

## Descripción de los Componentes

### 📱 Aplicaciones Frontend

#### 🌐 Web SPA (Single Page Application)
- **Tecnología**: React/Angular
- **Propósito**: Interfaz web para usuarios finales
- **Características**:
  - Interfaz responsive
  - Comunicación REST con la API
  - Autenticación JWT
  - Real-time updates vía SignalR

#### 📱 Mobile App
- **Tecnología**: React Native
- **Propósito**: Aplicación móvil multiplataforma
- **Características**:
  - Funcionalidad offline
  - Notificaciones push
  - Escaneado de códigos QR
  - Sincronización con la API

#### 💻 Desktop App
- **Tecnología**: WPF/Electron
- **Propósito**: Aplicación de escritorio para POS
- **Características**:
  - Integración con hardware (impresoras, lectores)
  - Funcionalidad offline
  - Sincronización por lotes
  - Interface nativa del SO

### 🚀 Núcleo SIFEN API

#### 🎯 REST API
- **Tecnología**: .NET 9
- **Propósito**: API principal del sistema
- **Responsabilidades**:
  - Endpoints REST para todas las operaciones
  - Autenticación y autorización
  - Validación de datos
  - Orquestación de servicios de negocio

#### ⏰ Background Jobs
- **Tecnología**: Hangfire
- **Propósito**: Procesamiento asíncrono
- **Responsabilidades**:
  - Procesamiento de documentos en lotes
  - Reintento de operaciones fallidas
  - Tareas programadas
  - Procesamiento en modo contingencia

#### ⚡ SignalR Hub
- **Tecnología**: SignalR
- **Propósito**: Comunicación en tiempo real
- **Responsabilidades**:
  - Notificaciones push a clientes
  - Actualizaciones de estado en tiempo real
  - Mensajería bidireccional
  - Gestión de conexiones

### ⚙️ Servicios de Negocio

#### 📄 XML Service
- **Propósito**: Generación de XMLs SIFEN
- **Responsabilidades**:
  - Generar XML según especificaciones SIFEN
  - Validar estructura XML
  - Aplicar transformaciones XSL
  - Cache de templates XML

#### ✍️ Signature Service
- **Propósito**: Firmado digital de documentos
- **Responsabilidades**:
  - Firmar XMLs con certificados digitales
  - Validar certificados
  - Gestionar almacén de certificados
  - Verificar firmas digitales

#### 📱 QR Service
- **Propósito**: Generación de códigos QR
- **Responsabilidades**:
  - Generar códigos QR según formato SIFEN
  - Configurar nivel de corrección de errores
  - Optimizar tamaño y calidad
  - Cache de códigos generados

#### 📋 PDF Service
- **Propósito**: Generación de PDFs KUDE
- **Responsabilidades**:
  - Generar PDFs según formato KUDE
  - Aplicar templates personalizados
  - Incluir códigos QR y firmas
  - Optimizar tamaño de archivos

#### 📧 Email Service
- **Propósito**: Envío de notificaciones por correo
- **Responsabilidades**:
  - Enviar facturas por correo
  - Notificaciones de estado
  - Templates de correo
  - Cola de envío con reintentos

#### 💾 Storage Service
- **Propósito**: Gestión de archivos
- **Responsabilidades**:
  - Almacenar PDFs y XMLs
  - Gestionar versiones de archivos
  - Compresión y optimización
  - Backup y recuperación

### 🏗️ Infraestructura

#### 🗄️ SQL Server
- **Propósito**: Base de datos principal
- **Características**:
  - Almacenamiento transaccional
  - Integridad referencial
  - Backup automático
  - Alta disponibilidad

#### ⚡ Redis Cache
- **Propósito**: Cache distribuido
- **Usos**:
  - Cache de sesiones
  - Cache de datos frecuentes
  - Lock distribuido
  - Message broker

#### 📁 Blob Storage
- **Propósito**: Almacenamiento de archivos
- **Contenido**:
  - PDFs generados
  - XMLs firmados
  - Certificados digitales
  - Logs de sistema

#### 📦 Message Queue
- **Propósito**: Cola de mensajes
- **Usos**:
  - Procesamiento asíncrono
  - Desacoplamiento de servicios
  - Garantía de entrega
  - Load balancing

### 🌐 Servicios Externos

#### 🏛️ SIFEN SET
- **Propósito**: Sistema de la SET
- **Comunicación**: SOAP/XML
- **Servicios**:
  - Envío de documentos
  - Consulta de estados
  - Eventos de cancelación
  - Validaciones

#### 📧 SMTP Server
- **Propósito**: Servidor de correo
- **Configuración**:
  - Relay SMTP
  - Autenticación
  - Cifrado TLS
  - Rate limiting

#### 🗝 Certificate Authority
- **Propósito**: Autoridad certificadora
- **Servicios**:
  - Emisión de certificados
  - Validación de certificados
  - Lista de revocación (CRL)
  - Timestamping

## Patrones de Comunicación

### 🔄 Comunicación Síncrona
- Frontend → REST API
- REST API → Servicios de Negocio
- Servicios → Base de Datos

### ⚡ Comunicación Asíncrona
- Background Jobs → Message Queue
- SignalR Hub → Clientes Frontend
- Email Service → SMTP Server

### 🔒 Seguridad
- JWT tokens para autenticación
- HTTPS para todas las comunicaciones
- Certificados digitales para firmado
- Rate limiting y throttling