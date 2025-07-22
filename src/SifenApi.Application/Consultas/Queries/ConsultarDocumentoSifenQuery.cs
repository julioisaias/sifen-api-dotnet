using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.Consultas.Queries;

public class ConsultarDocumentoSifenQuery : IRequest<ApiResponse<ConsultaDocumentoSifenResponse>>
{
    public string Cdc { get; }

    public ConsultarDocumentoSifenQuery(string cdc)
    {
        Cdc = cdc;
    }
}

public class ConsultaDocumentoSifenResponse
{
    public string Cdc { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string? RucReceptor { get; set; }
    public string? RazonSocialReceptor { get; set; }
    public decimal TotalGeneral { get; set; }
    public string? MensajeResultado { get; set; }
}