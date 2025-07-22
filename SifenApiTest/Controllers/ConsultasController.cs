using Microsoft.AspNetCore.Mvc;

namespace SifenApiTest.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ConsultasController : ControllerBase
{
    [HttpGet("documento/{cdc}")]
    public IActionResult ConsultarDocumento(string cdc)
    {
        var response = new DocumentoConsultaResponse
        {
            Cdc = cdc,
            Estado = "Aprobado",
            FechaEmision = DateTime.Now.AddDays(-1),
            RucEmisor = "80069590-1",
            RazonSocial = "EMPRESA DE PRUEBA S.A.",
            NumeroControl = "12345678901234567890",
            ProtocoloAutorizacion = "987654321098765"
        };
        
        return Ok(response);
    }

    [HttpGet("ruc/{ruc}")]
    public IActionResult ConsultarPorRuc(string ruc, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
    {
        var documentos = new List<DocumentoConsultaResponse>();
        for (int i = 1; i <= 5; i++)
        {
            documentos.Add(new DocumentoConsultaResponse
            {
                Cdc = $"01800695901001001000000123456789{i:D6}",
                Estado = "Aprobado",
                FechaEmision = DateTime.Now.AddDays(-i),
                RucEmisor = ruc,
                RazonSocial = "EMPRESA DE PRUEBA S.A.",
                NumeroControl = $"12345678901234567{i:D3}",
                ProtocoloAutorizacion = $"98765432109876{i}"
            });
        }
        
        return Ok(new { Data = documentos, Total = documentos.Count });
    }

    [HttpGet("lote/{loteId}")]
    public IActionResult ConsultarLote(Guid loteId)
    {
        var response = new LoteConsultaResponse
        {
            Id = loteId,
            Estado = "Procesado",
            FechaEnvio = DateTime.Now.AddHours(-2),
            TotalDocumentos = 3,
            DocumentosAprobados = 2,
            DocumentosRechazados = 1
        };
        
        return Ok(response);
    }
}

public class DocumentoConsultaResponse
{
    public string Cdc { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NumeroControl { get; set; } = string.Empty;
    public string ProtocoloAutorizacion { get; set; } = string.Empty;
}

public class LoteConsultaResponse
{
    public Guid Id { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; }
    public int TotalDocumentos { get; set; }
    public int DocumentosAprobados { get; set; }
    public int DocumentosRechazados { get; set; }
}