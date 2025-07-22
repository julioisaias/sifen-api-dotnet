using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.Consultas.Queries;

public class ConsultarLoteSifenQuery : IRequest<ApiResponse<ConsultaLoteResponse>>
{
    public string NumeroLote { get; }

    public ConsultarLoteSifenQuery(string numeroLote)
    {
        NumeroLote = numeroLote;
    }
}

public class ConsultaLoteResponse
{
    public string NumeroLote { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }
    public int CantidadDocumentos { get; set; }
    public List<DocumentoLoteDto> Documentos { get; set; } = new();
}

public class DocumentoLoteDto
{
    public string Cdc { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? MensajeError { get; set; }
}