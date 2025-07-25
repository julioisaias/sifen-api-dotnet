using SifenApi.Application.Common.Models;

namespace SifenApi.Application.Common.Interfaces;

public interface ISifenClient
{
    Task<SifenResponse> RecibeLoteAsync(
        List<string> xmlDocumentos,
        string ambiente,
        CancellationToken cancellationToken = default);
    
    Task<SifenResponse> RecibeAsync(
        string xmlDocumento,
        string ambiente,
        CancellationToken cancellationToken = default);
    
    Task<SifenResponse> EventoAsync(
        string xmlEvento,
        string ambiente,
        CancellationToken cancellationToken = default);
    
    Task<SifenConsultaResponse> ConsultaAsync(
        string cdc,
        string ambiente,
        CancellationToken cancellationToken = default);
    
    Task<SifenConsultaRucResponse> ConsultaRucAsync(
        string ruc,
        string ambiente,
        CancellationToken cancellationToken = default);
    
    Task<SifenConsultaLoteResponse> ConsultaLoteAsync(
        string numeroLote,
        string ambiente,
        CancellationToken cancellationToken = default);
}
