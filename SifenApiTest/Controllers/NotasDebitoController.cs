using Microsoft.AspNetCore.Mvc;

namespace SifenApiTest.Controllers;

[ApiController]
[Route("api/v1/notas-debito")]
public class NotasDebitoController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateNotaDebito([FromBody] CreateNotaDebitoRequest request)
    {
        var response = new NotaDebitoResponse
        {
            Id = Guid.NewGuid(),
            Cdc = "05800695901001001000000123456789012345",
            NumeroDocumento = "0000001",
            Estado = "Aprobado",
            FechaEmision = DateTime.Now,
            Total = request.Total,
            DocumentoAsociadoCdc = request.DocumentoAsociadoCdc
        };
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetNotaDebito(Guid id)
    {
        var response = new NotaDebitoResponse
        {
            Id = id,
            Cdc = "05800695901001001000000123456789012345",
            NumeroDocumento = "0000001",
            Estado = "Aprobado",
            FechaEmision = DateTime.Now,
            Total = 25000,
            DocumentoAsociadoCdc = "01800695901001001000000123456789012345"
        };
        
        return Ok(response);
    }
}

public class CreateNotaDebitoRequest
{
    public string DocumentoAsociadoCdc { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ItemDto> Items { get; set; } = new();
}

public class NotaDebitoResponse
{
    public Guid Id { get; set; }
    public string Cdc { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal Total { get; set; }
    public string DocumentoAsociadoCdc { get; set; } = string.Empty;
}