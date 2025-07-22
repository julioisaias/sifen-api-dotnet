using Microsoft.AspNetCore.Mvc;

namespace SifenApiTest.Controllers;

[ApiController]
[Route("api/v1/notas-credito")]
public class NotasCreditoController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateNotaCredito([FromBody] CreateNotaCreditoRequest request)
    {
        var response = new NotaCreditoResponse
        {
            Id = Guid.NewGuid(),
            Cdc = "04800695901001001000000123456789012345",
            NumeroDocumento = "0000001",
            Estado = "Aprobado",
            FechaEmision = DateTime.Now,
            Total = request.Total,
            DocumentoAsociadoCdc = request.DocumentoAsociadoCdc
        };
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetNotaCredito(Guid id)
    {
        var response = new NotaCreditoResponse
        {
            Id = id,
            Cdc = "04800695901001001000000123456789012345",
            NumeroDocumento = "0000001",
            Estado = "Aprobado",
            FechaEmision = DateTime.Now,
            Total = 50000,
            DocumentoAsociadoCdc = "01800695901001001000000123456789012345"
        };
        
        return Ok(response);
    }
}

public class CreateNotaCreditoRequest
{
    public string DocumentoAsociadoCdc { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ItemDto> Items { get; set; } = new();
}

public class NotaCreditoResponse
{
    public Guid Id { get; set; }
    public string Cdc { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal Total { get; set; }
    public string DocumentoAsociadoCdc { get; set; } = string.Empty;
}