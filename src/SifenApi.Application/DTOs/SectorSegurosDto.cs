namespace SifenApi.Application.DTOs;

public class SectorSegurosDto
{
    public string? CodigoAseguradora { get; set; }
    public string CodigoPoliza { get; set; } = string.Empty;
    public string NumeroPoliza { get; set; } = string.Empty;
    public int Vigencia { get; set; }
    public string VigenciaUnidad { get; set; } = string.Empty;
    public DateTime InicioVigencia { get; set; }
    public DateTime FinVigencia { get; set; }
    public string? CodigoInternoItem { get; set; }
}