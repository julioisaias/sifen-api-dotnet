namespace SifenApi.Application.DTOs;

public class SectorSupermercadosDto
{
    public string NombreCajero { get; set; } = string.Empty;
    public decimal Efectivo { get; set; }
    public decimal? Vuelto { get; set; }
    public decimal? Donacion { get; set; }
    public string? DonacionDescripcion { get; set; }
}