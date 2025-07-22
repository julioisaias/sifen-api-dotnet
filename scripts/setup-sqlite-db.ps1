# Script para configurar y poblar la base de datos SQLite

Write-Host "Configurando base de datos SQLite..." -ForegroundColor Green

# Cambiar al directorio raíz del proyecto
$rootPath = Split-Path -Parent $PSScriptRoot
Push-Location $rootPath

try {
    # Eliminar base de datos existente si existe
    $dbFile = "SifenApiDb_Dev.db"
    if (Test-Path $dbFile) {
        Write-Host "Eliminando base de datos existente..." -ForegroundColor Yellow
        Remove-Item $dbFile -Force
    }

    # Ejecutar migraciones
    Write-Host "Aplicando migraciones de Entity Framework..." -ForegroundColor Yellow
    dotnet ef database update --project src/SifenApi.Infrastructure --startup-project src/SifenApi.WebApi

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error al aplicar migraciones" -ForegroundColor Red
        exit 1
    }

    # Verificar que se creó la base de datos
    if (-not (Test-Path $dbFile)) {
        Write-Host "No se encontró el archivo de base de datos" -ForegroundColor Red
        exit 1
    }

    # Ejecutar script SQL para poblar datos
    Write-Host "Poblando base de datos con datos de prueba..." -ForegroundColor Yellow
    $scriptPath = Join-Path $PSScriptRoot "init-sqlite.sql"
    
    # Usar sqlite3 si está disponible
    if (Get-Command sqlite3 -ErrorAction SilentlyContinue) {
        Get-Content $scriptPath | sqlite3 $dbFile
    } else {
        Write-Host "sqlite3 no está instalado. Intentando con dotnet-ef..." -ForegroundColor Yellow
        
        # Como alternativa, crear un programa C# temporal para ejecutar el SQL
        $tempCs = @"
using Microsoft.Data.Sqlite;
using System;
using System.IO;

var connectionString = "Data Source=SifenApiDb_Dev.db";
using var connection = new SqliteConnection(connectionString);
connection.Open();

var sql = File.ReadAllText(@"scripts\init-sqlite.sql");
var commands = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);

foreach (var commandText in commands)
{
    if (!string.IsNullOrWhiteSpace(commandText))
    {
        using var command = connection.CreateCommand();
        command.CommandText = commandText.Trim();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ejecutando comando: {ex.Message}");
        }
    }
}

Console.WriteLine("Datos de prueba insertados correctamente!");
"@
        
        $tempFile = [System.IO.Path]::GetTempFileName() + ".cs"
        $tempCs | Out-File -FilePath $tempFile -Encoding UTF8
        
        dotnet run --project src/SifenApi.WebApi -- $tempFile
        Remove-Item $tempFile -Force
    }

    Write-Host "Base de datos SQLite configurada correctamente!" -ForegroundColor Green
    Write-Host "Archivo de base de datos: $dbFile" -ForegroundColor Cyan
    Write-Host "ContribuyenteId de prueba: 00000000-0000-0000-0000-000000000001" -ForegroundColor Cyan
    
} finally {
    Pop-Location
}