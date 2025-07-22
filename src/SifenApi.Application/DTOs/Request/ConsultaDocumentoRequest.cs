namespace SifenApi.Application.DTOs.Request;

public class ConsultaDocumentoRequest
{
    public string? Cdc { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? RucCliente { get; set; }
    public int? Estado { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}