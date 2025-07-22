using Microsoft.AspNetCore.Mvc;

namespace SifenApiTest.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FacturasController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateFactura([FromBody] CreateFacturaRequest request)
    {
        var response = new FacturaResponse
        {
            Id = Guid.NewGuid(),
            Cdc = "01800695901001001000000123456789012345",
            NumeroDocumento = "0000001",
            Estado = "Aprobado",
            FechaEmision = DateTime.Now,
            Total = request.Total
        };
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetFactura(Guid id)
    {
        var response = new FacturaResponse
        {
            Id = id,
            Cdc = "01800695901001001000000123456789012345",
            NumeroDocumento = "0000001",
            Estado = "Aprobado",
            FechaEmision = DateTime.Now,
            Total = 150000
        };
        
        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetFacturas([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var facturas = new List<FacturaResponse>();
        for (int i = 1; i <= size; i++)
        {
            facturas.Add(new FacturaResponse
            {
                Id = Guid.NewGuid(),
                Cdc = $"01800695901001001000000123456789{i:D6}",
                NumeroDocumento = $"{i:D7}",
                Estado = "Aprobado",
                FechaEmision = DateTime.Now.AddDays(-i),
                Total = 100000 + (i * 1000)
            });
        }
        
        return Ok(new { Data = facturas, Page = page, Size = size, Total = 100 });
    }

    [HttpPost("{id}/enviar-sifen")]
    public IActionResult EnviarASifen(Guid id)
    {
        return Ok(new { Message = "Factura enviada a SIFEN exitosamente", ProtocoloAutorizacion = "12345678901234567890" });
    }
}

public class CreateFacturaRequest
{
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public ClienteDto Cliente { get; set; } = new();
    public List<ItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class ClienteDto
{
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

public class ItemDto
{
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
}

public class FacturaResponse
{
    public Guid Id { get; set; }
    public string Cdc { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal Total { get; set; }
}