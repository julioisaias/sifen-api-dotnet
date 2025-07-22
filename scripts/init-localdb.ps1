# Script para inicializar LocalDB con datos de prueba

Write-Host "Inicializando base de datos de prueba..." -ForegroundColor Green

# Verificar si LocalDB está instalado
try {
    $localdb = & sqllocaldb info 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "LocalDB no está instalado. Por favor instale SQL Server LocalDB." -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "Error al verificar LocalDB: $_" -ForegroundColor Red
    exit 1
}

# Crear o verificar instancia
Write-Host "Verificando instancia mssqllocaldb..." -ForegroundColor Yellow
& sqllocaldb create mssqllocaldb 2>&1 | Out-Null
& sqllocaldb start mssqllocaldb

# Ejecutar script SQL usando sqlcmd
Write-Host "Ejecutando script de datos de prueba..." -ForegroundColor Yellow
$scriptPath = Join-Path $PSScriptRoot "init-test-db.sql"

# Primero crear la base de datos si no existe
$createDbScript = @"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SifenApiDb_Dev')
BEGIN
    CREATE DATABASE SifenApiDb_Dev;
END
"@

# Ejecutar creación de base de datos
$createDbScript | sqlcmd -S "(localdb)\mssqllocaldb" -E

# Ejecutar script de datos
sqlcmd -S "(localdb)\mssqllocaldb" -d "SifenApiDb_Dev" -E -i $scriptPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "Base de datos inicializada correctamente!" -ForegroundColor Green
    Write-Host "ContribuyenteId de prueba: 00000000-0000-0000-0000-000000000001" -ForegroundColor Cyan
} else {
    Write-Host "Error al ejecutar el script SQL" -ForegroundColor Red
    exit 1
}