using MediatR;
using SifenApi.Application.DTOs.Request;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Queries;

public class GetDocumentosQuery : IRequest<ApiResponse<PagedResponse<DocumentoElectronicoResponse>>>
{
    public ConsultaDocumentoRequest Request { get; }
    public Guid ContribuyenteId { get; }

    public GetDocumentosQuery(ConsultaDocumentoRequest request, Guid contribuyenteId)
    {
        Request = request;
        ContribuyenteId = contribuyenteId;
    }
}