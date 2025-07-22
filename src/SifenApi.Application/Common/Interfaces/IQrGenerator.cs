namespace SifenApi.Application.Common.Interfaces;

public interface IQrGenerator
{
    Task<string> GenerateQrAsync(
        string cdc,
        decimal totalGeneral,
        string? numeroRuc,
        Dictionary<string, string>? parametrosAdicionales = null,
        CancellationToken cancellationToken = default);
    
    byte[] GenerateQrImage(string qrData);
}
