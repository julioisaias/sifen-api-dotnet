namespace SifenApi.Infrastructure.Security;

public class CertificateStore
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContribuyenteId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string Thumbprint { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public DateTime NotBefore { get; set; }
    public DateTime ExpirationDate { get; set; }
    public byte[] EncryptedData { get; set; } = Array.Empty<byte>();
    public string EncryptedPassword { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}