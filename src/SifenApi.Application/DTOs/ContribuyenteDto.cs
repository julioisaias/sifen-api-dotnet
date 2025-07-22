namespace SifenApi.Application.DTOs;

public class ContribuyenteDto
{
    public Guid Id { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreFantasia { get; set; }
    public string TipoContribuyente { get; set; } = string.Empty;
    public int TipoRegimen { get; set; }
    public List<ActividadEconomicaDto> ActividadesEconomicas { get; set; } = new();
    public List<EstablecimientoDto> Establecimientos { get; set; } = new();
    public bool Activo { get; set; }
}

public class EstablecimientoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Denominacion { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}