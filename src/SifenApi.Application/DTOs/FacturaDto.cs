using System.Text.Json.Serialization;

namespace SifenApi.Application.DTOs;

public class FacturaDto
{
    public int TipoDocumento { get; set; }
    public string Establecimiento { get; set; } = string.Empty;
    public string PuntoExpedicion { get; set; } = string.Empty;
    public string? Numero { get; set; }
    public string? Descripcion { get; set; }
    public string? Observacion { get; set; }
    public DateTime Fecha { get; set; }
    public int TipoEmision { get; set; } = 1;
    public int TipoTransaccion { get; set; }
    public int TipoImpuesto { get; set; } = 1;
    public string Moneda { get; set; } = "PYG";
    public int CondicionAnticipo { get; set; } = 1;
    public int? CondicionTipoCambio { get; set; }
    public decimal? DescuentoGlobal { get; set; }
    public decimal? AnticipoGlobal { get; set; }
    public decimal? Cambio { get; set; }
    
    public ClienteDto Cliente { get; set; } = new();
    public UsuarioDto? Usuario { get; set; }
    public DatosFacturaDto? Factura { get; set; }
    public CondicionDto Condicion { get; set; } = new();
    public List<ItemDto> Items { get; set; } = new();
    
    // Datos opcionales según tipo de negocio
    public SectorEnergiaElectricaDto? SectorEnergiaElectrica { get; set; }
    public SectorSegurosDto? SectorSeguros { get; set; }
    public SectorSupermercadosDto? SectorSupermercados { get; set; }
    public SectorAdicionalDto? SectorAdicional { get; set; }
    public DetalleTransporteDto? DetalleTransporte { get; set; }
    public ComplementariosDto? Complementarios { get; set; }
    public DocumentoAsociadoDto? DocumentoAsociado { get; set; }
}