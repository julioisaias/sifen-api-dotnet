using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class SendDocumentoToSifenCommand : IRequest<ApiResponse<bool>>
{
    public Guid DocumentoId { get; }

    public SendDocumentoToSifenCommand(Guid documentoId)
    {
        DocumentoId = documentoId;
    }
}