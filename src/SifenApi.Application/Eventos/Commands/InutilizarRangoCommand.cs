using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.Eventos.Commands;

public class InutilizarRangoCommand : IRequest<ApiResponse<EventoResponse>>
{
    public int TipoDocumento { get; }
    public string Establecimiento { get; }
    public string PuntoExpedicion { get; }
    public int NumeroDesde { get; }
    public int NumeroHasta { get; }
    public string Motivo { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }

    public InutilizarRangoCommand(
        int tipoDocumento,
        string establecimiento,
        string puntoExpedicion,
        int numeroDesde,
        int numeroHasta,
        string motivo,
        Guid contribuyenteId,
        string userId)
    {
        TipoDocumento = tipoDocumento;
        Establecimiento = establecimiento;
        PuntoExpedicion = puntoExpedicion;
        NumeroDesde = numeroDesde;
        NumeroHasta = numeroHasta;
        Motivo = motivo;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
    }
}