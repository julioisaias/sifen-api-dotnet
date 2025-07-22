using MediatR;

namespace SifenApi.Application.DocumentosElectronicos.Queries;

public class GetXmlQuery : IRequest<string>
{
    public Guid DocumentoId { get; }
    public Guid ContribuyenteId { get; }

    public GetXmlQuery(Guid documentoId, Guid contribuyenteId)
    {
        DocumentoId = documentoId;
        ContribuyenteId = contribuyenteId;
    }
}