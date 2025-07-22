using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class SendLoteToSifenCommand : IRequest<ApiResponse<LoteResponse>>
{
    public List<Guid> DocumentoIds { get; }

    public SendLoteToSifenCommand(List<Guid> documentoIds)
    {
        DocumentoIds = documentoIds;
    }
}

public class LoteResponse
{
    public string NumeroLote { get; set; } = string.Empty;
    public int CantidadDocumentos { get; set; }
    public List<DocumentoLoteResult> Resultados { get; set; } = new();
}

public class DocumentoLoteResult
{
    public Guid DocumentoId { get; set; }
    public string? Cdc { get; set; }
    public bool Aprobado { get; set; }
    public string? Mensaje { get; set; }
}