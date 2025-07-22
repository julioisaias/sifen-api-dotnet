namespace SifenApi.Application.DTOs;

public class ActividadEconomicaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
}