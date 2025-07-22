using MediatR;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class CreateNotaRemisionCommand : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public FacturaDto NotaRemision { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }
    public int MotivoEmision { get; }
    public int TipoResponsable { get; }
    public int? Kms { get; }

    public CreateNotaRemisionCommand(
        FacturaDto notaRemision, 
        Guid contribuyenteId, 
        string userId, 
        int motivoEmision,
        int tipoResponsable,
        int? kms)
    {
        NotaRemision = notaRemision;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
        MotivoEmision = motivoEmision;
        TipoResponsable = tipoResponsable;
        Kms = kms;
    }
}