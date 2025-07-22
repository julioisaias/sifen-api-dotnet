using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Queries;

public class GetDocumentoByIdQuery : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public Guid DocumentoId { get; }
    public Guid ContribuyenteId { get; }

    public GetDocumentoByIdQuery(Guid documentoId, Guid contribuyenteId)
    {
        DocumentoId = documentoId;
        ContribuyenteId = contribuyenteId;
    }
}