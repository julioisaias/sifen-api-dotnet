namespace SifenApi.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendDocumentoElectronicoAsync(
        string to,
        string subject,
        string cdc,
        byte[] kudePdf,
        string? xmlContent = null,
        CancellationToken cancellationToken = default);
    
    Task SendNotificationAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}