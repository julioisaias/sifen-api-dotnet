using Microsoft.AspNetCore.Mvc;

namespace SifenApiTest.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventosController : ControllerBase
{
    [HttpPost("cancelar")]
    public IActionResult CancelarDocumento([FromBody] CancelarDocumentoRequest request)
    {
        var response = new EventoResponse
        {
            Id = Guid.NewGuid(),
            TipoEvento = "Cancelacion",
            Estado = "Aprobado",
            FechaEvento = DateTime.Now,
            DocumentoId = request.DocumentoId,
            Motivo = request.Motivo
        };
        
        return Ok(response);
    }

    [HttpPost("inutilizar")]
    public IActionResult InutilizarRango([FromBody] InutilizarRangoRequest request)
    {
        var response = new EventoResponse
        {
            Id = Guid.NewGuid(),
            TipoEvento = "Inutilizacion",
            Estado = "Aprobado",
            FechaEvento = DateTime.Now,
            Motivo = request.Motivo
        };
        
        return Ok(response);
    }

    [HttpPost("conformidad")]
    public IActionResult RegistrarConformidad([FromBody] ConformidadRequest request)
    {
        var response = new EventoResponse
        {
            Id = Guid.NewGuid(),
            TipoEvento = "Conformidad",
            Estado = "Aprobado",
            FechaEvento = DateTime.Now,
            DocumentoId = request.DocumentoId
        };
        
        return Ok(response);
    }
}

public class CancelarDocumentoRequest
{
    public Guid DocumentoId { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public class InutilizarRangoRequest
{
    public int RangoInicio { get; set; }
    public int RangoFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public class ConformidadRequest
{
    public Guid DocumentoId { get; set; }
}

public class EventoResponse
{
    public Guid Id { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public Guid? DocumentoId { get; set; }
    public string Motivo { get; set; } = string.Empty;
}