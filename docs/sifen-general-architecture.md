# SIFEN - Arquitectura General del Sistema

## Diagrama de Arquitectura General

Este diagrama muestra la arquitectura completa del sistema SIFEN con todas sus capas y componentes principales.

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#e1f5fe',
    'primaryTextColor': '#01579b',
    'primaryBorderColor': '#0288d1',
    'lineColor': '#0288d1',
    'secondaryColor': '#f3e5f5',
    'tertiaryColor': '#e8f5e8',
    'background': '#ffffff',
    'mainBkg': '#e1f5fe',
    'secondBkg': '#f3e5f5',
    'tertiaryBkg': '#e8f5e8'
  }
}}%%
graph TB
    subgraph "🖥️ Aplicaciones Cliente"
        WEB["🌐 Aplicación Web<br/><small>React/Angular</small>"]
        MOB["📱 App Mobile<br/><small>React Native</small>"]
        ERP["🏢 Sistema ERP<br/><small>Integración</small>"]
        POS["🛒 Punto de Venta<br/><small>Desktop</small>"]
    end
    
    subgraph "🔐 Gateway de Seguridad"
        NGINX["⚡ Nginx/Load Balancer<br/><small>Alta disponibilidad</small>"]
        WAF["🛡️ Web Application Firewall<br/><small>Protección</small>"]
    end
    
    subgraph "🚀 SIFEN API"
        subgraph "📡 Capa de Presentación"
            API["🎯 REST API Controllers<br/><small>Endpoints</small>"]
            MW["🔧 Middleware<br/><small>Interceptores</small>"]
            AUTH["🔑 Authentication<br/><small>JWT/OAuth</small>"]
        end
        
        subgraph "⚙️ Capa de Aplicación"
            CMD["📝 Commands/CQRS<br/><small>Escritura</small>"]
            QRY["📊 Queries<br/><small>Lectura</small>"]
            VAL["✅ Validators<br/><small>Validación</small>"]
            MAP["🗺️ AutoMapper<br/><small>Mapping</small>"]
        end
        
        subgraph "🏗️ Capa de Dominio"
            ENT["🏛️ Entities<br/><small>Entidades</small>"]
            VO["💎 Value Objects<br/><small>Objetos de Valor</small>"]
            EVT["⚡ Domain Events<br/><small>Eventos</small>"]
            INT["🔌 Interfaces<br/><small>Contratos</small>"]
        end
        
        subgraph "🔧 Capa de Infraestructura"
            REPO["📚 Repositories<br/><small>Persistencia</small>"]
            XML["📄 XML Generator<br/><small>Generador</small>"]
            SIGN["✍️ XML Signer<br/><small>Firmado</small>"]
            QR["📱 QR Generator<br/><small>Códigos QR</small>"]
            KUDE["📋 KUDE Generator<br/><small>PDF</small>"]
            SIFEN["🏛️ SIFEN Client<br/><small>Integración</small>"]
        end
    end
    
    subgraph "☁️ Servicios Externos"
        DB[("🗄️ SQL Server<br/><small>Base de Datos</small>")]
        REDIS[("⚡ Redis Cache<br/><small>Cache</small>")]
        BLOB["📦 Blob Storage<br/><small>Archivos</small>"]
        SMTP["📧 Email Server<br/><small>Correos</small>"]
        SET["🏛️ SIFEN SET<br/><small>Gobierno</small>"]
    end
    
    WEB --> NGINX
    MOB --> NGINX
    ERP --> NGINX
    POS --> NGINX
    
    NGINX --> WAF
    WAF --> API
    
    API --> MW
    MW --> AUTH
    AUTH --> CMD
    AUTH --> QRY
    
    CMD --> VAL
    QRY --> MAP
    CMD --> ENT
    QRY --> ENT
    
    ENT --> VO
    ENT --> EVT
    ENT --> INT
    
    REPO --> DB
    CMD --> REPO
    QRY --> REPO
    
    CMD --> XML
    XML --> SIGN
    CMD --> QR
    CMD --> KUDE
    CMD --> SIFEN
    
    SIFEN --> SET
    REPO --> REDIS
    KUDE --> BLOB
    CMD --> SMTP

    classDef clientStyle fill:#e3f2fd,stroke:#1976d2,stroke-width:2px,color:#0d47a1
    classDef gatewayStyle fill:#fff3e0,stroke:#f57c00,stroke-width:2px,color:#e65100
    classDef apiStyle fill:#e8f5e8,stroke:#388e3c,stroke-width:2px,color:#1b5e20
    classDef externalStyle fill:#fce4ec,stroke:#c2185b,stroke-width:2px,color:#880e4f
    
    class WEB,MOB,ERP,POS clientStyle
    class NGINX,WAF gatewayStyle
    class API,MW,AUTH,CMD,QRY,VAL,MAP,ENT,VO,EVT,INT,REPO,XML,SIGN,QR,KUDE,SIFEN apiStyle
    class DB,REDIS,BLOB,SMTP,SET externalStyle
```

## Descripción de las Capas

### 🖥️ Aplicaciones Cliente
- **Aplicación Web**: Frontend web desarrollado en React/Angular
- **App Mobile**: Aplicación móvil desarrollada en React Native
- **Sistema ERP**: Integración con sistemas de gestión empresarial
- **Punto de Venta**: Aplicación de escritorio para POS

### 🔐 Gateway de Seguridad
- **Nginx/Load Balancer**: Balanceador de carga y proxy inverso
- **Web Application Firewall**: Protección contra ataques web

### 🚀 SIFEN API
#### 📡 Capa de Presentación
- **REST API Controllers**: Endpoints de la API REST
- **Middleware**: Interceptores para logging, validación, etc.
- **Authentication**: Autenticación JWT/OAuth

#### ⚙️ Capa de Aplicación
- **Commands/CQRS**: Comandos para operaciones de escritura
- **Queries**: Consultas para operaciones de lectura
- **Validators**: Validadores de datos
- **AutoMapper**: Mapeo entre objetos

#### 🏗️ Capa de Dominio
- **Entities**: Entidades del dominio
- **Value Objects**: Objetos de valor
- **Domain Events**: Eventos del dominio
- **Interfaces**: Contratos e interfaces

#### 🔧 Capa de Infraestructura
- **Repositories**: Acceso a datos
- **XML Generator**: Generación de XML SIFEN
- **XML Signer**: Firmado digital de documentos
- **QR Generator**: Generación de códigos QR
- **KUDE Generator**: Generación de PDFs KUDE
- **SIFEN Client**: Cliente para comunicación con SIFEN

### ☁️ Servicios Externos
- **SQL Server**: Base de datos principal
- **Redis Cache**: Cache distribuido
- **Blob Storage**: Almacenamiento de archivos
- **Email Server**: Servidor de correo SMTP
- **SIFEN SET**: Sistema Electrónico de Tributación del gobierno