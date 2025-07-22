using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Queries;

public class GetDocumentoByCdcQuery : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public string Cdc { get; }

    public GetDocumentoByCdcQuery(string cdc)
    {
        Cdc = cdc;
    }
}