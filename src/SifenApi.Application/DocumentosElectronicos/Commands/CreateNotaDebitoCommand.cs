using MediatR;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class CreateNotaDebitoCommand : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public FacturaDto NotaDebito { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }
    public int MotivoEmision { get; }

    public CreateNotaDebitoCommand(FacturaDto notaDebito, Guid contribuyenteId, string userId, int motivoEmision)
    {
        NotaDebito = notaDebito;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
        MotivoEmision = motivoEmision;
    }
}