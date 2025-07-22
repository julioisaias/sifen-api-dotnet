using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SifenApi.Application.Consultas.Queries;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.WebApi.Controllers.V1;

// [Authorize] // Temporarily disabled for testing
public class ConsultasController : BaseApiController
{
    /// <summary>
    /// Consulta un RUC en SIFEN
    /// </summary>
    [HttpGet("ruc/{ruc}")]
    [ProducesResponseType(typeof(ApiResponse<ConsultaRucResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarRuc(string ruc)
    {
        var query = new ConsultarRucSifenQuery(ruc);
        var result = await Mediator.Send(query);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }

    /// <summary>
    /// Consulta un documento en SIFEN por CDC
    /// </summary>
    [HttpGet("documento/{cdc}")]
    [ProducesResponseType(typeof(ApiResponse<ConsultaDocumentoSifenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarDocumento(string cdc)
    {
        var query = new ConsultarDocumentoSifenQuery(cdc);
        var result = await Mediator.Send(query);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }

    /// <summary>
    /// Consulta un lote en SIFEN
    /// </summary>
    [HttpGet("lote/{numeroLote}")]
    [ProducesResponseType(typeof(ApiResponse<ConsultaLoteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarLote(string numeroLote)
    {
        var query = new ConsultarLoteSifenQuery(numeroLote);
        var result = await Mediator.Send(query);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }
}