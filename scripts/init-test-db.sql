-- Script para inicializar la base de datos con datos de prueba
-- Compatible con SQL Server y SQLite

-- Eliminar datos existentes (opcional, comentar si no se desea)
-- DELETE FROM DocumentosElectronicos;
-- DELETE FROM Timbrados;
-- DELETE FROM Establecimientos;
-- DELETE FROM ContribuyenteActividad;
-- DELETE FROM Contribuyentes;
-- DELETE FROM ActividadesEconomicas;
-- DELETE FROM Clientes;

-- 1. Insertar Actividades Económicas
INSERT INTO ActividadesEconomicas (Id, Codigo, Descripcion, CreatedAt, CreatedBy)
SELECT '11111111-1111-1111-1111-111111111111', '46201', 'VENTA AL POR MAYOR DE EQUIPOS INFORMÁTICOS', GETDATE(), 'system'
WHERE NOT EXISTS (SELECT 1 FROM ActividadesEconomicas WHERE Id = '11111111-1111-1111-1111-111111111111');

INSERT INTO ActividadesEconomicas (Id, Codigo, Descripcion, CreatedAt, CreatedBy)
SELECT '22222222-2222-2222-2222-222222222222', '47411', 'VENTA AL POR MENOR DE COMPUTADORAS Y EQUIPOS PERIFÉRICOS', GETDATE(), 'system'
WHERE NOT EXISTS (SELECT 1 FROM ActividadesEconomicas WHERE Id = '22222222-2222-2222-2222-222222222222');

-- 2. Insertar Contribuyente de prueba con ID fijo
INSERT INTO Contribuyentes (
    Id, 
    Ruc, 
    RazonSocial, 
    NombreFantasia, 
    Email, 
    Telefono, 
    Direccion, 
    Ciudad, 
    Departamento,
    TipoContribuyente,
    TipoRegimen,
    Activo, 
    CreatedAt, 
    CreatedBy
)
SELECT 
    '00000000-0000-0000-0000-000000000001',
    '80069590-1',
    'EMPRESA DEMO S.A.',
    'DEMO TECH',
    'info@empresademo.com.py',
    '021-123456',
    'Av. España 123',
    'Asunción',
    'Central',
    1, -- Persona Jurídica
    1, -- Régimen General
    1,
    GETDATE(),
    'system'
WHERE NOT EXISTS (SELECT 1 FROM Contribuyentes WHERE Id = '00000000-0000-0000-0000-000000000001');

-- 3. Insertar relación Contribuyente-Actividad
INSERT INTO ContribuyenteActividad (ContribuyentesId, ActividadesEconomicasId)
SELECT '00000000-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111'
WHERE NOT EXISTS (
    SELECT 1 FROM ContribuyenteActividad 
    WHERE ContribuyentesId = '00000000-0000-0000-0000-000000000001' 
    AND ActividadesEconomicasId = '11111111-1111-1111-1111-111111111111'
);

-- 4. Insertar Establecimiento
INSERT INTO Establecimientos (
    Id,
    Codigo,
    Denominacion,
    Direccion,
    NumeroCasa,
    Departamento,
    DepartamentoDescripcion,
    Distrito,
    DistritoDescripcion,
    Ciudad,
    CiudadDescripcion,
    Telefono,
    Email,
    ContribuyenteId,
    Activo,
    CreatedAt,
    CreatedBy
)
SELECT
    '33333333-3333-3333-3333-333333333333',
    '001',
    'Casa Matriz',
    'Av. España 123',
    '123',
    11, -- Central
    'CENTRAL',
    1, -- Asunción
    'ASUNCION',
    1,
    'ASUNCION',
    '021-123456',
    'matriz@empresademo.com.py',
    '00000000-0000-0000-0000-000000000001',
    1,
    GETDATE(),
    'system'
WHERE NOT EXISTS (SELECT 1 FROM Establecimientos WHERE Id = '33333333-3333-3333-3333-333333333333');

-- 5. Insertar Timbrado vigente
INSERT INTO Timbrados (
    Id,
    Numero,
    FechaInicio,
    FechaVencimiento,
    NumeroInicial,
    NumeroFinal,
    ContribuyenteId,
    EstablecimientoId,
    PuntoExpedicion,
    Activo,
    CreatedAt,
    CreatedBy
)
SELECT
    '44444444-4444-4444-4444-444444444444',
    '12345678',
    DATEADD(MONTH, -1, GETDATE()),
    DATEADD(YEAR, 1, GETDATE()),
    1,
    9999999,
    '00000000-0000-0000-0000-000000000001',
    '33333333-3333-3333-3333-333333333333',
    '001',
    1,
    GETDATE(),
    'system'
WHERE NOT EXISTS (SELECT 1 FROM Timbrados WHERE Id = '44444444-4444-4444-4444-444444444444');

-- 6. Insertar clientes de prueba
INSERT INTO Clientes (
    Id,
    Ruc,
    RazonSocial,
    NombreFantasia,
    Direccion,
    Telefono,
    Email,
    TipoDocumento,
    NumeroDocumento,
    EsContribuyente,
    Activo,
    CreatedAt,
    CreatedBy
)
SELECT
    '55555555-5555-5555-5555-555555555555',
    '80056234-5',
    'CLIENTE DEMO S.R.L.',
    'CLIENTE TECH',
    'Mcal. López 456',
    '021-987654',
    'info@clientedemo.com.py',
    NULL,
    NULL,
    1,
    1,
    GETDATE(),
    'system'
WHERE NOT EXISTS (SELECT 1 FROM Clientes WHERE Id = '55555555-5555-5555-5555-555555555555');

-- Cliente no contribuyente
INSERT INTO Clientes (
    Id,
    Ruc,
    RazonSocial,
    NombreFantasia,
    Direccion,
    Telefono,
    Email,
    TipoDocumento,
    NumeroDocumento,
    EsContribuyente,
    Activo,
    CreatedAt,
    CreatedBy
)
SELECT
    '66666666-6666-6666-6666-666666666666',
    NULL,
    'Juan Pérez',
    NULL,
    'Barrio San Pablo',
    '0981-123456',
    'juan@email.com',
    1, -- Cédula
    '4567890',
    0,
    1,
    GETDATE(),
    'system'
WHERE NOT EXISTS (SELECT 1 FROM Clientes WHERE Id = '66666666-6666-6666-6666-666666666666');

-- Mostrar resumen de datos insertados
SELECT 'Datos de prueba insertados correctamente' as Mensaje;
SELECT COUNT(*) as TotalContribuyentes FROM Contribuyentes;
SELECT COUNT(*) as TotalEstablecimientos FROM Establecimientos;
SELECT COUNT(*) as TotalTimbrados FROM Timbrados WHERE Activo = 1;
SELECT COUNT(*) as TotalClientes FROM Clientes;