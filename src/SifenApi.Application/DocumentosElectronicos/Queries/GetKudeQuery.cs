using MediatR;

namespace SifenApi.Application.DocumentosElectronicos.Queries;

public class GetKudeQuery : IRequest<byte[]>
{
    public Guid DocumentoId { get; }
    public Guid ContribuyenteId { get; }

    public GetKudeQuery(Guid documentoId, Guid contribuyenteId)
    {
        DocumentoId = documentoId;
        ContribuyenteId = contribuyenteId;
    }
}