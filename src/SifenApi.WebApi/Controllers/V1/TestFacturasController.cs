using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SifenApi.Application.DocumentosElectronicos.Commands;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Request;
using SifenApi.Application.DTOs.Response;
using SifenApi.Domain.Enums;

namespace SifenApi.WebApi.Controllers.V1;

[AllowAnonymous]
[ApiController]
[Route("api/v1/test-facturas")]
public class TestFacturasController : BaseApiController
{
    private readonly ILogger<TestFacturasController> _logger;
    
    // Usar un ContribuyenteId fijo para pruebas
    private readonly Guid TestContribuyenteId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly string TestUserId = "test-user";

    public TestFacturasController(ILogger<TestFacturasController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva factura electrónica (ENDPOINT DE PRUEBA)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TestFacturaRequest request)
    {
        try
        {
            // Convertir el request simplificado a FacturaDto completo
            var facturaDto = new FacturaDto
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
                TipoCambio = null,
                Condicion = new CondicionVentaDto
                {
                    Tipo = (int)CondicionVenta.Contado,
                    Entregas = new List<EntregaDto>
                    {
                        new EntregaDto
                        {
                            Tipo = 1,
                            Monto = request.Total,
                            Moneda = "PYG"
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
                    Email = "cliente@test.com"
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
                    IvaBase = 100,
                    Lote = null,
                    Vencimiento = null,
                    NumeroSerie = null,
                    NumeroPedido = null,
                    NumeroSeguimiento = null,
                    RegistroSenave = null,
                    RegistroEntidadComercial = null
                }).ToList()
            };

            // Usar un contribuyente fijo para pruebas
            var command = new CreateFacturaCommand(facturaDto, TestContribuyenteId, TestUserId);
            var result = await Mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Factura de prueba creada exitosamente: {Id}", result.Data?.Id);
                return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
            }

            _logger.LogError("Error al crear factura de prueba: {Message}", result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al crear factura de prueba");
            return StatusCode(500, ApiResponse<DocumentoElectronicoResponse>.Fail(
                "Error interno del servidor", 
                ex.Message));
        }
    }

    /// <summary>
    /// Obtiene una factura por ID (ENDPOINT DE PRUEBA)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentoElectronicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetDocumentoByIdQuery(id, TestContribuyenteId);
        var result = await Mediator.Send(query);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }
}

// DTOs simplificados para testing
public class TestFacturaRequest
{
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public TestClienteDto Cliente { get; set; } = new();
    public List<TestItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class TestClienteDto
{
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

public class TestItemDto
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal? Descuento { get; set; }
}