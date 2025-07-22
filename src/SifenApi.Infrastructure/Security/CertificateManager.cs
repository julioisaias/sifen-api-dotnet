using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Security;

public class CertificateManager
{
    private readonly SifenDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CertificateManager> _logger;

    public CertificateManager(
        SifenDbContext context,
        IConfiguration configuration,
        ILogger<CertificateManager> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<X509Certificate2?> GetCertificateAsync(
        Guid contribuyenteId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar certificado en base de datos
            var certificateData = await _context.Set<CertificateStore>()
                .FirstOrDefaultAsync(c => c.ContribuyenteId == contribuyenteId && c.IsActive, cancellationToken);

            if (certificateData == null)
            {
                _logger.LogWarning("No se encontró certificado activo para contribuyente {ContribuyenteId}", contribuyenteId);
                return null;
            }

            // Validar vigencia
            if (certificateData.ExpirationDate < DateTime.UtcNow)
            {
                _logger.LogWarning("Certificado expirado para contribuyente {ContribuyenteId}", contribuyenteId);
                certificateData.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
                return null;
            }

            // Desencriptar certificado
            var certificateBytes = DecryptCertificate(certificateData.EncryptedData);
            var password = DecryptPassword(certificateData.EncryptedPassword);

            return new X509Certificate2(certificateBytes, password,
                X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo certificado para contribuyente {ContribuyenteId}", contribuyenteId);
            return null;
        }
    }

    public async Task<bool> StoreCertificateAsync(
        Guid contribuyenteId,
        byte[] certificateData,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar certificado
            var certificate = new X509Certificate2(certificateData, password);
            
            // Desactivar certificados anteriores
            var oldCertificates = await _context.Set<CertificateStore>()
                .Where(c => c.ContribuyenteId == contribuyenteId && c.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var oldCert in oldCertificates)
            {
                oldCert.IsActive = false;
            }

            // Encriptar y almacenar nuevo certificado
            var encryptedData = EncryptCertificate(certificateData);
            var encryptedPassword = EncryptPassword(password);

            var certificateStore = new CertificateStore
            {
                ContribuyenteId = contribuyenteId,
                SubjectName = certificate.Subject,
                Thumbprint = certificate.Thumbprint,
                SerialNumber = certificate.SerialNumber ?? string.Empty,
                IssuerName = certificate.Issuer,
                NotBefore = certificate.NotBefore,
                ExpirationDate = certificate.NotAfter,
                EncryptedData = encryptedData,
                EncryptedPassword = encryptedPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<CertificateStore>().Add(certificateStore);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Certificado almacenado exitosamente para contribuyente {ContribuyenteId}", contribuyenteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error almacenando certificado para contribuyente {ContribuyenteId}", contribuyenteId);
            return false;
        }
    }

    private byte[] EncryptCertificate(byte[] data)
    {
        // Implementar encriptación con AES
        // Por simplicidad, retornamos los mismos datos
        // En producción, usar encriptación real
        return data;
    }

    private byte[] DecryptCertificate(byte[] encryptedData)
    {
        // Implementar desencriptación con AES
        // Por simplicidad, retornamos los mismos datos
        // En producción, usar desencriptación real
        return encryptedData;
    }

    private string EncryptPassword(string password)
    {
        // Implementar encriptación de password
        // En producción, usar encriptación real
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private string DecryptPassword(string encryptedPassword)
    {
        // Implementar desencriptación de password
        // En producción, usar desencriptación real
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPassword));
    }
}