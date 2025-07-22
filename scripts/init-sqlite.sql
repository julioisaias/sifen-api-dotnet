-- Script para inicializar SQLite con datos de prueba

-- 1. Insertar Actividades Económicas
INSERT OR IGNORE INTO ActividadesEconomicas (Id, Codigo, Descripcion, CreatedAt, CreatedBy)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '46201', 'VENTA AL POR MAYOR DE EQUIPOS INFORMÁTICOS', datetime('now'), 'system'),
    ('22222222-2222-2222-2222-222222222222', '47411', 'VENTA AL POR MENOR DE COMPUTADORAS Y EQUIPOS PERIFÉRICOS', datetime('now'), 'system');

-- 2. Insertar Contribuyente de prueba con ID fijo
INSERT OR IGNORE INTO Contribuyentes (
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
VALUES (
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
    datetime('now'),
    'system'
);

-- 3. Insertar relación Contribuyente-Actividad
INSERT OR IGNORE INTO ContribuyenteActividad (ContribuyentesId, ActividadesEconomicasId)
VALUES ('00000000-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111');

-- 4. Insertar Establecimiento
INSERT OR IGNORE INTO Establecimientos (
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
VALUES (
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
    datetime('now'),
    'system'
);

-- 5. Insertar Timbrado vigente
INSERT OR IGNORE INTO Timbrados (
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
VALUES (
    '44444444-4444-4444-4444-444444444444',
    '12345678',
    date('now', '-1 month'),
    date('now', '+1 year'),
    1,
    9999999,
    '00000000-0000-0000-0000-000000000001',
    '33333333-3333-3333-3333-333333333333',
    '001',
    1,
    datetime('now'),
    'system'
);

-- 6. Insertar clientes de prueba
INSERT OR IGNORE INTO Clientes (
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
VALUES (
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
    datetime('now'),
    'system'
);

-- Cliente no contribuyente
INSERT OR IGNORE INTO Clientes (
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
VALUES (
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
    datetime('now'),
    'system'
);