using QRCoder;
using SifenApi.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace SifenApi.Infrastructure.Services.Qr;

public class QrGenerator : IQrGenerator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QrGenerator> _logger;
    private readonly string _urlConsulta;

    public QrGenerator(IConfiguration configuration, ILogger<QrGenerator> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _urlConsulta = _configuration["Sifen:UrlConsultaPublica"] ?? "https://ekuatia.set.gov.py/consultas";
    }

    public async Task<string> GenerateQrAsync(
        string cdc,
        decimal totalGeneral,
        string? numeroRuc,
        Dictionary<string, string>? parametrosAdicionales = null,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                // Generar hash para seguridad
                var dataToHash = $"{cdc}{totalGeneral:F2}{numeroRuc ?? ""}";
                var hash = GenerateHash(dataToHash);

                // Construir URL del QR
                var qrData = new StringBuilder($"{_urlConsulta}?");
                qrData.Append($"nVersion=150");
                qrData.Append($"&Id={cdc}");
                qrData.Append($"&dFeEmiDE={DateTime.UtcNow:yyyy-MM-dd}");
                qrData.Append($"&dTotGralOpe={totalGeneral:F2}");
                
                if (!string.IsNullOrEmpty(numeroRuc))
                {
                    qrData.Append($"&dRucRec={numeroRuc}");
                }
                
                qrData.Append($"&dTotIVA={totalGeneral * 0.0909m:F2}"); // Aproximación del IVA
                qrData.Append($"&cItems=1");
                qrData.Append($"&DigestValue={hash.Substring(0, 8)}");

                // Agregar parámetros adicionales si existen
                if (parametrosAdicionales != null)
                {
                    foreach (var param in parametrosAdicionales)
                    {
                        qrData.Append($"&{param.Key}={param.Value}");
                    }
                }

                return qrData.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando datos del QR para CDC {Cdc}", cdc);
                throw;
            }
        }, cancellationToken);
    }

    public byte[] GenerateQrImage(string qrData)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20); // 20 pixels por módulo
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando imagen QR");
            throw;
        }
    }

    private string GenerateHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
