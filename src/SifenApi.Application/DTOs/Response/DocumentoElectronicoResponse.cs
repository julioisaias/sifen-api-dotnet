namespace SifenApi.Application.DTOs.Response;

public class DocumentoElectronicoResponse
{
    public Guid Id { get; set; }
    public string? Cdc { get; set; }
    public int TipoDocumento { get; set; }
    public string TipoDocumentoDescripcion { get; set; } = string.Empty;
    public string Establecimiento { get; set; } = string.Empty;
    public string PuntoExpedicion { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public ClienteResponseDto? Cliente { get; set; }
    public string? QrData { get; set; }
    public string? NumeroControlSifen { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? KudeUrl { get; set; }
    public string? XmlUrl { get; set; }
}

public class ClienteResponseDto
{
    public string? Ruc { get; set; }
    public string? RazonSocial { get; set; }
    public string? DocumentoNumero { get; set; }
    public string? Nombre { get; set; }
}

public class EventoResponse
{
    public Guid Id { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public Guid DocumentoElectronicoId { get; set; }
    public string? XmlEvento { get; set; }
    public string? Respuesta { get; set; }
}

