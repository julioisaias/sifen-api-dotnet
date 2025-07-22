using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SifenApi.Application.DocumentosElectronicos.Commands;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;
using SifenApi.Application.Common.Interfaces;

namespace SifenApi.WebApi.Controllers.V1;

// [Authorize] // Temporarily disabled for testing
public class NotasCreditoController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;

    public NotasCreditoController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>
    /// Crea una nueva nota de crédito electrónica
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] NotaCreditoRequest request)
    {
        var command = new CreateNotaCreditoCommand(
            request.NotaCredito, 
            _currentUser.ContribuyenteId, 
            _currentUser.UserId,
            request.MotivoEmision);
        
        var result = await Mediator.Send(command);

        if (result.Success)
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);

        return BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Implementación similar a FacturasController
        return Ok();
    }
}

public class NotaCreditoRequest
{
    public FacturaDto NotaCredito { get; set; } = new();
    public int MotivoEmision { get; set; }
}