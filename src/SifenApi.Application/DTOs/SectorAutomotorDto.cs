namespace SifenApi.Application.DTOs;

public class SectorAutomotorDto
{
    public int Tipo { get; set; }
    public string? Chasis { get; set; }
    public string? Color { get; set; }
    public int? Potencia { get; set; }
    public int? CapacidadMotor { get; set; }
    public int? CapacidadPasajeros { get; set; }
    public decimal? PesoBruto { get; set; }
    public decimal? PesoNeto { get; set; }
    public int? TipoCombustible { get; set; }
    public string? TipoCombustibleDescripcion { get; set; }
    public string? NumeroMotor { get; set; }
    public decimal? CapacidadTraccion { get; set; }
    public int? Año { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? Cilindradas { get; set; }
}