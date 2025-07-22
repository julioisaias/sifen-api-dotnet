using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SifenApi.Application.Eventos.Commands;
using SifenApi.Application.DTOs.Events;
using SifenApi.Application.DTOs.Response;
using SifenApi.Application.Common.Interfaces;

namespace SifenApi.WebApi.Controllers.V1;

// [Authorize] // Temporarily disabled for testing
public class EventosController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;

    public EventosController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>
    /// Cancela un documento electrónico
    /// </summary>
    [HttpPost("cancelacion")]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancelar([FromBody] CancelacionDto cancelacion)
    {
        var command = new CancelarDocumentoCommand(
            cancelacion.Cdc,
            cancelacion.Motivo,
            _currentUser.ContribuyenteId,
            _currentUser.UserId);
        
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Inutiliza un rango de numeración
    /// </summary>
    [HttpPost("inutilizacion")]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Inutilizar([FromBody] InutilizacionDto inutilizacion)
    {
        var command = new InutilizarRangoCommand(
            inutilizacion.TipoDocumento,
            inutilizacion.Establecimiento,
            inutilizacion.Punto,
            inutilizacion.Desde,
            inutilizacion.Hasta,
            inutilizacion.Motivo,
            _currentUser.ContribuyenteId,
            _currentUser.UserId);
        
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Registra conformidad con un documento
    /// </summary>
    [HttpPost("conformidad")]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarConformidad([FromBody] ConformidadDto conformidad)
    {
        var command = new RegistrarConformidadCommand(
            conformidad.Cdc,
            conformidad.TipoConformidad,
            conformidad.FechaRecepcion,
            _currentUser.ContribuyenteId,
            _currentUser.UserId);
        
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Registra disconformidad con un documento
    /// </summary>
    [HttpPost("disconformidad")]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarDisconformidad([FromBody] DisconformidadDto disconformidad)
    {
        var command = new RegistrarDisconformidadCommand(
            disconformidad.Cdc,
            disconformidad.Motivo,
            _currentUser.ContribuyenteId,
            _currentUser.UserId);
        
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}