using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.WebApi;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SifenDbContext>();
        
        // Aplicar migraciones
        await context.Database.MigrateAsync();
        
        // Solo insertar datos si no existen
        if (!await context.Contribuyentes.AnyAsync())
        {
            await SeedDataAsync(context);
        }
    }
    
    private static async Task SeedDataAsync(SifenDbContext context)
    {
        // 1. Actividades Económicas
        var actividad1 = new ActividadEconomica
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Codigo = "46201",
            Descripcion = "VENTA AL POR MAYOR DE EQUIPOS INFORMÁTICOS"
        };
        actividad1.SetCreatedBy("system");
        
        var actividad2 = new ActividadEconomica
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Codigo = "47411",
            Descripcion = "VENTA AL POR MENOR DE COMPUTADORAS Y EQUIPOS PERIFÉRICOS"
        };
        actividad2.SetCreatedBy("system");
        
        context.ActividadesEconomicas.AddRange(actividad1, actividad2);
        
        // 2. Contribuyente
        var contribuyente = new Contribuyente(
            "80069590-1",
            "EMPRESA DEMO S.A.",
            "DEMO TECH",
            TipoContribuyente.PersonaJuridica,
            1
        );
        contribuyente.Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        contribuyente.SetContactInfo("info@empresademo.com.py", "021-123456", "Av. España 123", "Asunción", "Central");
        contribuyente.SetCreatedBy("system");
        contribuyente.AgregarActividadEconomica(actividad1);
        
        context.Contribuyentes.Add(contribuyente);
        
        // 3. Establecimiento
        var establecimiento = new Establecimiento(
            "001",
            "Casa Matriz",
            "Av. España 123",
            contribuyente.Id
        );
        establecimiento.Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        establecimiento.SetUbicacion(
            "123",
            null,
            null,
            11,
            "CENTRAL",
            1,
            "ASUNCION",
            1,
            "ASUNCION"
        );
        establecimiento.SetContactInfo("021-123456", "matriz@empresademo.com.py");
        establecimiento.SetCreatedBy("system");
        
        context.Establecimientos.Add(establecimiento);
        
        // 4. Timbrado
        var timbrado = new Timbrado(
            "12345678",
            DateTime.Now.AddMonths(-1),
            DateTime.Now.AddYears(1),
            1,
            9999999,
            contribuyente.Id,
            establecimiento.Id,
            "001"
        );
        timbrado.Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        timbrado.SetCreatedBy("system");
        
        context.Timbrados.Add(timbrado);
        
        // 5. Clientes
        var cliente1 = Cliente.CrearContribuyente(
            "80056234-5",
            "CLIENTE DEMO S.R.L.",
            "Mcal. López 456",
            "021-987654",
            "info@clientedemo.com.py"
        );
        cliente1.Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        cliente1.SetCreatedBy("system");
        
        var cliente2 = Cliente.CrearNoContribuyente(
            TipoDocumentoIdentidad.CedulaParaguaya,
            "4567890",
            "Juan Pérez",
            "Barrio San Pablo",
            "0981-123456",
            "juan@email.com"
        );
        cliente2.Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        cliente2.SetCreatedBy("system");
        
        context.Clientes.AddRange(cliente1, cliente2);
        
        // Guardar todos los cambios
        await context.SaveChangesAsync();
        
        Console.WriteLine("Base de datos inicializada con datos de prueba.");
        Console.WriteLine($"ContribuyenteId: {contribuyente.Id}");
    }
}