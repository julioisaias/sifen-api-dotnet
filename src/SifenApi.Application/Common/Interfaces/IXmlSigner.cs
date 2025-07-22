namespace SifenApi.Application.Common.Interfaces;

public interface IXmlSigner
{
    Task<string> SignXmlAsync(
        string xml, 
        string certificatePath, 
        string certificatePassword,
        CancellationToken cancellationToken = default);
    
    Task<string> SignXmlWithStoredCertificateAsync(
        string xml,
        Guid contribuyenteId,
        CancellationToken cancellationToken = default);
}
