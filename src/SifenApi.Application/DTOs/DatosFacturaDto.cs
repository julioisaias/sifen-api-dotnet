namespace SifenApi.Application.DTOs;

public class DatosFacturaDto
{
    public int Presencia { get; set; } = 1;
    public DateTime? FechaEnvio { get; set; }
    public DncpFacturaDto? Dncp { get; set; }
}

public class DncpFacturaDto
{
    public string Modalidad { get; set; } = string.Empty;
    public int Entidad { get; set; }
    public int Año { get; set; }
    public int Secuencia { get; set; }
    public DateTime Fecha { get; set; }
}