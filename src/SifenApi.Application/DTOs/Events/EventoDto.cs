namespace SifenApi.Application.DTOs.Events;

public class CancelacionDto
{
    public string Cdc { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}

public class InutilizacionDto
{
    public int TipoDocumento { get; set; }
    public string Establecimiento { get; set; } = string.Empty;
    public string Punto { get; set; } = string.Empty;
    public int Desde { get; set; }
    public int Hasta { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public class ConformidadDto
{
    public string Cdc { get; set; } = string.Empty;
    public int TipoConformidad { get; set; }
    public DateTime FechaRecepcion { get; set; }
}

public class DisconformidadDto
{
    public string Cdc { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}

public class DesconocimientoDto
{
    public string Cdc { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaRecepcion { get; set; }
    public int TipoReceptor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Ruc { get; set; }
    public int? DocumentoTipo { get; set; }
    public string? DocumentoNumero { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public class NotificacionDto
{
    public string Cdc { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaRecepcion { get; set; }
    public int TipoReceptor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Ruc { get; set; }
    public int? DocumentoTipo { get; set; }
    public string? DocumentoNumero { get; set; }
    public decimal TotalPYG { get; set; }
}