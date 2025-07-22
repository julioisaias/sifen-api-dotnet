using MediatR;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class CreateFacturaCommand : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public FacturaDto Factura { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }

    public CreateFacturaCommand(FacturaDto factura, Guid contribuyenteId, string userId)
    {
        Factura = factura;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
    }
}