using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.Eventos.Commands;

public class CancelarDocumentoCommand : IRequest<ApiResponse<EventoResponse>>
{
    public string Cdc { get; }
    public string Motivo { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }

    public CancelarDocumentoCommand(string cdc, string motivo, Guid contribuyenteId, string userId)
    {
        Cdc = cdc;
        Motivo = motivo;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
    }
}

