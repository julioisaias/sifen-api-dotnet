namespace SifenApi.Application.Common.Interfaces;

public interface IKudeGenerator
{
    Task<byte[]> GenerateKudePdfAsync(
        string xmlDocumento,
        string qrData,
        CancellationToken cancellationToken = default);
    
    Task<string> GenerateKudeHtmlAsync(
        string xmlDocumento,
        string qrData,
        CancellationToken cancellationToken = default);
}