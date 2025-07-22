using MediatR;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class CreateNotaCreditoCommand : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public FacturaDto NotaCredito { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }
    public int MotivoEmision { get; }

    public CreateNotaCreditoCommand(FacturaDto notaCredito, Guid contribuyenteId, string userId, int motivoEmision)
    {
        NotaCredito = notaCredito;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
        MotivoEmision = motivoEmision;
    }
}