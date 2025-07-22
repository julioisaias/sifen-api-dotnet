using MediatR;
using SifenApi.Application.DTOs.Response;
using SifenApi.Application.DTOs.Events;

namespace SifenApi.Application.Eventos.Commands;

public class RegistrarDesconocimientoCommand : IRequest<ApiResponse<EventoResponse>>
{
    public DesconocimientoDto Desconocimiento { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }

    public RegistrarDesconocimientoCommand(
        DesconocimientoDto desconocimiento,
        Guid contribuyenteId,
        string userId)
    {
        Desconocimiento = desconocimiento;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
    }
}