using MediatR;
using SifenApi.Application.DTOs.Response;
using SifenApi.Application.DTOs.Events;

namespace SifenApi.Application.Eventos.Commands;

public class RegistrarNotificacionCommand : IRequest<ApiResponse<EventoResponse>>
{
    public NotificacionDto Notificacion { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }

    public RegistrarNotificacionCommand(
        NotificacionDto notificacion,
        Guid contribuyenteId,
        string userId)
    {
        Notificacion = notificacion;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
    }
}