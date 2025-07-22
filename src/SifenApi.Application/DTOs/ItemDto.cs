namespace SifenApi.Application.DTOs;

public class ItemDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Observacion { get; set; }
    public int? PartidaArancelaria { get; set; }
    public string? Ncm { get; set; }
    public int UnidadMedida { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal? Cambio { get; set; }
    public decimal? Descuento { get; set; }
    public decimal? Anticipo { get; set; }
    public string? Pais { get; set; } = "PRY";
    public string? PaisDescripcion { get; set; } = "Paraguay";
    public int? Tolerancia { get; set; }
    public int? ToleranciaCantidad { get; set; }
    public int? ToleranciaPorcentaje { get; set; }
    public string? CdcAnticipo { get; set; }
    public DncpDto? Dncp { get; set; }
    public int IvaTipo { get; set; } = 1;
    public decimal IvaBase { get; set; } = 100;
    public decimal Iva { get; set; } = 10;
    public string? Lote { get; set; }
    public DateTime? Vencimiento { get; set; }
    public string? NumeroSerie { get; set; }
    public string? NumeroPedido { get; set; }
    public string? NumeroSeguimiento { get; set; }
    public ImportadorDto? Importador { get; set; }
    public string? RegistroSenave { get; set; }
    public string? RegistroEntidadComercial { get; set; }
    public SectorAutomotorDto? SectorAutomotor { get; set; }
}