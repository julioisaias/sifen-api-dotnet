# SifenApi - Deployment Guide

## Requisitos del Sistema

### Software Requerido
- .NET 9.0 Runtime
- SQL Server 2019 o superior
- IIS (para deployment en Windows) o Docker

### Hardware Recomendado
- CPU: 2+ cores
- RAM: 4GB mínimo, 8GB recomendado
- Storage: 50GB mínimo
- Red: Conexión estable a Internet para comunicación con SIFEN

## Deployment con Docker

### 1. Preparar el entorno
```bash
# Clonar el repositorio
git clone <repository-url>
cd SifenApi

# Construir y ejecutar con docker-compose
docker-compose up -d
```

### 2. Configurar la base de datos
```bash
# Ejecutar migraciones
docker exec -it sifenapi_sifenapi_1 dotnet ef database update
```

## Deployment en IIS

### 1. Publicar la aplicación
```bash
dotnet publish src/SifenApi.WebApi/SifenApi.WebApi.csproj -c Release -o ./publish
```

### 2. Configurar IIS
1. Crear un nuevo Application Pool
2. Establecer .NET CLR Version a "No Managed Code"
3. Crear un nuevo Website apuntando a la carpeta publish
4. Configurar binding en puerto 80/443

### 3. Configurar SSL
- Instalar certificado SSL válido
- Configurar binding HTTPS en puerto 443
- Redirigir HTTP a HTTPS

## Variables de Entorno

### Configuración de Base de Datos
```
ConnectionStrings__DefaultConnection=Server=localhost;Database=SifenApiDb;Trusted_Connection=true;
```

### Configuración de SIFEN
```
Sifen__BaseUrl=https://sifen.set.gov.py/de/ws/
Sifen__Environment=test # o production
Sifen__CertificatePath=/path/to/certificate.p12
Sifen__CertificatePassword=your-password
```

### Configuración de Logging
```
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
```

## Monitoreo y Logs

### Health Checks
- URL: `/health`
- Verifica conectividad a base de datos y servicios externos

### Logs
- Ubicación: `./logs/` (configurable)
- Rotación automática diaria
- Niveles: Error, Warning, Information, Debug

## Backup y Recuperación

### Base de Datos
```sql
-- Backup
BACKUP DATABASE SifenApiDb TO DISK = 'C:\Backup\SifenApiDb.bak'

-- Restore
RESTORE DATABASE SifenApiDb FROM DISK = 'C:\Backup\SifenApiDb.bak'
```

### Certificados
- Mantener backup seguro de certificados digitales
- Verificar fecha de expiración regularmente

## Seguridad

### Firewall
- Puerto 80/443: Abierto para tráfico HTTP/HTTPS
- Puerto 1433: Restringido solo para conexión de base de datos local
- Puertos adicionales: Cerrados por defecto

### Certificados
- Usar solo certificados válidos para comunicación con SIFEN
- Almacenar certificados de forma segura
- Implementar rotación de certificados

## Troubleshooting

### Problemas Comunes

#### Error de conexión a SIFEN
- Verificar conectividad de red
- Validar certificados digitales
- Comprobar configuración de URLs

#### Error de base de datos
- Verificar string de conexión
- Comprobar permisos de usuario
- Validar estado del servicio SQL Server

#### Performance Issues
- Monitorear uso de CPU y memoria
- Verificar índices de base de datos
- Revisar logs de aplicación