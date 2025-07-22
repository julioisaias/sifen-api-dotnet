namespace SifenApi.Application.DTOs;

public class SectorEnergiaElectricaDto
{
    public string NumeroMedidor { get; set; } = string.Empty;
    public int CodigoActividad { get; set; }
    public string CodigoCategoria { get; set; } = string.Empty;
    public decimal LecturaAnterior { get; set; }
    public decimal LecturaActual { get; set; }
}