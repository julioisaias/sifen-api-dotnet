using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SifenApi.Application.DocumentosElectronicos.Commands;
using SifenApi.Application.DocumentosElectronicos.Queries;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Request;
using SifenApi.Application.DTOs.Response;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Domain.Enums;

namespace SifenApi.WebApi.Controllers.V1;

[AllowAnonymous] // Temporarily allow anonymous access for testing
public class FacturasController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;

    public FacturasController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>
    /// Crea una nueva factura electrónica (formato simplificado para pruebas)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFacturaSimpleRequest request)
    {
        // Convertir el request simplificado a FacturaDto completo
        var factura = ConvertToFacturaDto(request);
        
        // Para pruebas: si no hay autenticación, usar valores por defecto
        var contribuyenteId = _currentUser.ContribuyenteId;
        var userId = _currentUser.UserId;
        
        if (contribuyenteId == Guid.Empty)
        {
            // Usar el contribuyente de prueba
            contribuyenteId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            userId = "test-user";
        }
        
        var command = new CreateFacturaCommand(factura, contribuyenteId, userId);
        var result = await Mediator.Send(command);

        if (result.Success)
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);

        return BadRequest(result);
    }
    
    /// <summary>
    /// Crea una nueva factura electrónica (formato completo)
    /// </summary>
    [HttpPost("completa")]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompleta([FromBody] FacturaDto factura)
    {
        // Para pruebas: si no hay autenticación, usar valores por defecto
        var contribuyenteId = _currentUser.ContribuyenteId;
        var userId = _currentUser.UserId;
        
        if (contribuyenteId == Guid.Empty)
        {
            // Usar el contribuyente de prueba
            contribuyenteId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            userId = "test-user";
        }
        
        var command = new CreateFacturaCommand(factura, contribuyenteId, userId);
        var result = await Mediator.Send(command);

        if (result.Success)
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);

        return BadRequest(result);
    }

    /// <summary>
    /// Obtiene una factura por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var contribuyenteId = _currentUser.ContribuyenteId == Guid.Empty 
            ? Guid.Parse("00000000-0000-0000-0000-000000000001") 
            : _currentUser.ContribuyenteId;
            
        var query = new GetDocumentoByIdQuery(id, contribuyenteId);
        var result = await Mediator.Send(query);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }

    /// <summary>
    /// Obtiene una factura por CDC
    /// </summary>
    [HttpGet("cdc/{cdc}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCdc(string cdc)
    {
        var query = new GetDocumentoByCdcQuery(cdc);
        var result = await Mediator.Send(query);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }

    /// <summary>
    /// Busca facturas con filtros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<DocumentoElectronicoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] ConsultaDocumentoRequest request)
    {
        var contribuyenteId = _currentUser.ContribuyenteId == Guid.Empty 
            ? Guid.Parse("00000000-0000-0000-0000-000000000001") 
            : _currentUser.ContribuyenteId;
            
        var query = new GetDocumentosQuery(request, contribuyenteId);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Descarga el KUDE (PDF) de una factura
    /// </summary>
    [HttpGet("{id:guid}/kude")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetKude(Guid id)
    {
        var contribuyenteId = _currentUser.ContribuyenteId == Guid.Empty 
            ? Guid.Parse("00000000-0000-0000-0000-000000000001") 
            : _currentUser.ContribuyenteId;
            
        var query = new GetKudeQuery(id, contribuyenteId);
        var pdf = await Mediator.Send(query);
        
        if (pdf == null || pdf.Length == 0)
            return NotFound();

        return File(pdf, "application/pdf", $"KUDE_{id}.pdf");
    }

    /// <summary>
    /// Descarga el XML de una factura
    /// </summary>
    [HttpGet("{id:guid}/xml")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetXml(Guid id)
    {
        var contribuyenteId = _currentUser.ContribuyenteId == Guid.Empty 
            ? Guid.Parse("00000000-0000-0000-0000-000000000001") 
            : _currentUser.ContribuyenteId;
            
        var query = new GetXmlQuery(id, contribuyenteId);
        var xml = await Mediator.Send(query);
        
        if (string.IsNullOrEmpty(xml))
            return NotFound();

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        return File(bytes, "application/xml", $"DE_{id}.xml");
    }

    /// <summary>
    /// Envía una factura a SIFEN
    /// </summary>
    [HttpPost("{id:guid}/enviar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnviarSifen(Guid id)
    {
        var command = new SendDocumentoToSifenCommand(id);
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Envía múltiples facturas a SIFEN en lote
    /// </summary>
    [HttpPost("lote/enviar")]
    [ProducesResponseType(typeof(ApiResponse<LoteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnviarLoteSifen([FromBody] List<Guid> documentoIds)
    {
        var command = new SendLoteToSifenCommand(documentoIds);
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
    
    private FacturaDto ConvertToFacturaDto(CreateFacturaSimpleRequest request)
    {
        return new FacturaDto
        {
            TipoDocumento = (int)TipoDocumento.FacturaElectronica,
            Establecimiento = "001",
            PuntoExpedicion = "001",
            Numero = null, // Se generará automáticamente
            Fecha = DateTime.Now,
            TipoEmision = (int)TipoEmision.Normal,
            TipoTransaccion = (int)TipoTransaccion.VentaMercaderia,
            TipoImpuesto = (int)TipoImpuesto.Iva,
            Moneda = "PYG",
            Condicion = new CondicionDto
            {
                Tipo = (int)CondicionVenta.Contado,
                Entregas = new List<EntregaDto>
                {
                    new EntregaDto
                    {
                        Tipo = 1,
                        Monto = request.Total,
                        Moneda = "PYG",
                        Cambio = 0
                    }
                }
            },
            Cliente = new ClienteDto
            {
                Contribuyente = true,
                Ruc = request.Cliente.Ruc,
                RazonSocial = request.Cliente.RazonSocial,
                NombreFantasia = request.Cliente.RazonSocial,
                Direccion = request.Cliente.Direccion,
                TipoOperacion = (int)TipoOperacion.B2B,
                Pais = "PRY",
                PaisDescripcion = "Paraguay",
                Telefono = "021-000000",
                Email = "cliente@test.com",
                Departamento = 11,
                DepartamentoDescripcion = "CENTRAL",
                Distrito = 1,
                DistritoDescripcion = "ASUNCION",
                Ciudad = 1,
                CiudadDescripcion = "ASUNCION"
            },
            Usuario = new UsuarioDto
            {
                DocumentoTipo = (int)TipoDocumentoIdentidad.CedulaParaguaya,
                DocumentoNumero = "1234567",
                Nombre = "Usuario Test",
                Cargo = "Cajero"
            },
            Items = request.Items.Select(i => new ItemDto
            {
                Codigo = "001",
                Descripcion = i.Descripcion,
                UnidadMedida = 77, // UNI - Unidad
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Descuento = i.Descuento ?? 0,
                DescuentoGlobal = 0,
                Anticipo = 0,
                IvaTipo = (int)TipoIva.Gravado,
                Iva = 10,
                IvaBase = 100
            }).ToList()
        };
    }
}