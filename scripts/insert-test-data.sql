-- Script para insertar datos de prueba en la base de datos SifenApi
-- Este script inserta un contribuyente, establecimiento, actividad económica y timbrado para pruebas

-- 1. Insertar Actividad Económica
INSERT INTO ActividadesEconomicas (Id, Codigo, Descripcion, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), '46201', 'VENTA AL POR MAYOR DE EQUIPOS INFORMÁTICOS', GETDATE(), 'system'),
    (NEWID(), '47411', 'VENTA AL POR MENOR DE COMPUTADORAS Y EQUIPOS PERIFÉRICOS', GETDATE(), 'system');

-- 2. Insertar Contribuyente de prueba
DECLARE @ContribuyenteId UNIQUEIDENTIFIER = NEWID();
INSERT INTO Contribuyentes (Id, Ruc, RazonSocial, NombreFantasia, Email, Telefono, Direccion, Ciudad, Departamento, Activo, CreatedAt, CreatedBy)
VALUES 
    (@ContribuyenteId, '80069590-1', 'EMPRESA DEMO S.A.', 'DEMO TECH', 'info@empresademo.com.py', '021-123456', 'Av. España 123', 'Asunción', 'Central', 1, GETDATE(), 'system');

-- 3. Insertar relación Contribuyente-Actividad
DECLARE @ActividadId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM ActividadesEconomicas WHERE Codigo = '46201');
INSERT INTO ContribuyenteActividad (ContribuyentesId, ActividadesEconomicasId)
VALUES (@ContribuyenteId, @ActividadId);

-- 4. Insertar Establecimiento
DECLARE @EstablecimientoId UNIQUEIDENTIFIER = NEWID();
INSERT INTO Establecimientos (Id, Codigo, Denominacion, Direccion, Telefono, Email, ContribuyenteId, Activo, CreatedAt, CreatedBy)
VALUES 
    (@EstablecimientoId, '001', 'Casa Matriz', 'Av. España 123', '021-123456', 'matriz@empresademo.com.py', @ContribuyenteId, 1, GETDATE(), 'system');

-- 5. Insertar Timbrado vigente
INSERT INTO Timbrados (Id, Numero, FechaInicio, FechaVencimiento, NumeroInicial, NumeroFinal, ContribuyenteId, EstablecimientoId, PuntoExpedicion, Activo, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), '12345678', DATEADD(MONTH, -1, GETDATE()), DATEADD(YEAR, 1, GETDATE()), 1, 999999, @ContribuyenteId, @EstablecimientoId, '001', 1, GETDATE(), 'system');

-- 6. Insertar algunos clientes de prueba
INSERT INTO Clientes (Id, Ruc, RazonSocial, NombreFantasia, Direccion, Telefono, Email, TipoDocumento, NumeroDocumento, EsContribuyente, Activo, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), '80056234-5', 'CLIENTE DEMO S.R.L.', 'CLIENTE TECH', 'Mcal. López 456', '021-987654', 'info@clientedemo.com.py', NULL, NULL, 1, 1, GETDATE(), 'system'),
    (NEWID(), NULL, 'Juan Pérez', NULL, 'Barrio San Pablo', '0981-123456', 'juan@email.com', 1, '4567890', 0, 1, GETDATE(), 'system');

-- Mostrar IDs creados para referencia
SELECT 'ContribuyenteId' as Tipo, @ContribuyenteId as Id
UNION ALL
SELECT 'EstablecimientoId', @EstablecimientoId;