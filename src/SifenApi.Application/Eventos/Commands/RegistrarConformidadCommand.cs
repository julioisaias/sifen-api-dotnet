using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.Eventos.Commands;

public class RegistrarConformidadCommand : IRequest<ApiResponse<EventoResponse>>
{
    public string Cdc { get; }
    public int TipoConformidad { get; }
    public DateTime FechaRecepcion { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }

    public RegistrarConformidadCommand(
        string cdc,
        int tipoConformidad,
        DateTime fechaRecepcion,
        Guid contribuyenteId,
        string userId)
    {
        Cdc = cdc;
        TipoConformidad = tipoConformidad;
        FechaRecepcion = fechaRecepcion;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
    }
}