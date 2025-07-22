using SifenApi.Application.Common.Models;

namespace SifenApi.Application.Common.Interfaces;

public interface ISifenClient
{
    Task<SifenResponse> RecibeLoteAsync(
        List<string> xmlDocumentos,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default);
    
    Task<SifenResponse> RecibeAsync(
        string xmlDocumento,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default);
    
    Task<SifenResponse> EventoAsync(
        string xmlEvento,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default);
    
    Task<SifenConsultaResponse> ConsultaAsync(
        string cdc,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default);
    
    Task<SifenConsultaRucResponse> ConsultaRucAsync(
        string ruc,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default);
    
    Task<SifenConsultaLoteResponse> ConsultaLoteAsync(
        string numeroLote,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default);
}
