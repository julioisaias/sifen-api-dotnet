using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.WebApi.Controllers.V1;

[AllowAnonymous]
[ApiController]
[Route("api/v1/diagnostic")]
public class DiagnosticController : ControllerBase
{
    private readonly SifenDbContext _context;
    private readonly ILogger<DiagnosticController> _logger;

    public DiagnosticController(SifenDbContext context, ILogger<DiagnosticController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            var result = new
            {
                DatabaseExists = await _context.Database.CanConnectAsync(),
                Tables = new
                {
                    Contribuyentes = await _context.Contribuyentes.CountAsync(),
                    Establecimientos = await _context.Establecimientos.CountAsync(),
                    Timbrados = await _context.Timbrados.CountAsync(),
                    Clientes = await _context.Clientes.CountAsync(),
                    ActividadesEconomicas = await _context.ActividadesEconomicas.CountAsync()
                },
                TestContribuyente = await _context.Contribuyentes
                    .Where(c => c.Id == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                    .Select(c => new { c.Id, c.Ruc, c.RazonSocial })
                    .FirstOrDefaultAsync()
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database");
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpPost("test-simple")]
    public IActionResult TestSimple([FromBody] dynamic data)
    {
        try
        {
            _logger.LogInformation("Received data: {Data}", System.Text.Json.JsonSerializer.Serialize(data));
            return Ok(new { message = "Data received successfully", receivedData = data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test-simple");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}