using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Extensions.Logging;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Infrastructure.Security;

namespace SifenApi.Infrastructure.Services.Xml;

public class XmlSigner : IXmlSigner
{
    private readonly ILogger<XmlSigner> _logger;
    private readonly CertificateManager _certificateManager;

    public XmlSigner(ILogger<XmlSigner> logger, CertificateManager certificateManager)
    {
        _logger = logger;
        _certificateManager = certificateManager;
    }

    public async Task<string> SignXmlAsync(
        string xml,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                // Cargar certificado
                var certificate = new X509Certificate2(certificatePath, certificatePassword, 
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                return SignXmlWithCertificate(xml, certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error firmando XML con certificado desde {Path}", certificatePath);
                throw;
            }
        }, cancellationToken);
    }

    public async Task<string> SignXmlWithStoredCertificateAsync(
        string xml,
        Guid contribuyenteId,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(async () =>
        {
            try
            {
                // Obtener certificado almacenado
                var certificate = await _certificateManager.GetCertificateAsync(contribuyenteId, cancellationToken);
                if (certificate == null)
                    throw new InvalidOperationException($"No se encontró certificado para contribuyente {contribuyenteId}");

                return SignXmlWithCertificate(xml, certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error firmando XML con certificado almacenado para contribuyente {ContribuyenteId}", contribuyenteId);
                throw;
            }
        }, cancellationToken);
    }

    private string SignXmlWithCertificate(string xml, X509Certificate2 certificate)
    {
        // Validar certificado
        if (certificate.NotAfter < DateTime.UtcNow)
            throw new InvalidOperationException("El certificado ha expirado");

        // Cargar XML
        var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        xmlDoc.LoadXml(xml);

        // Obtener el elemento DE o gEve para firmar
        XmlElement? elementToSign = null;
        var deNodes = xmlDoc.GetElementsByTagName("DE");
        if (deNodes.Count > 0)
        {
            elementToSign = deNodes[0] as XmlElement;
        }
        else
        {
            var eveNodes = xmlDoc.GetElementsByTagName("gEve");
            if (eveNodes.Count > 0)
            {
                elementToSign = eveNodes[0] as XmlElement;
            }
        }

        if (elementToSign == null)
            throw new InvalidOperationException("No se encontró elemento DE o gEve para firmar");

        // Crear firma
        var signedXml = new SignedXml(xmlDoc)
        {
            SigningKey = certificate.GetRSAPrivateKey() ?? throw new InvalidOperationException("No se pudo obtener la clave privada del certificado")
        };

        // Configurar referencia
        var reference = new Reference($"#{elementToSign.GetAttribute("Id")}");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(reference);

        // Agregar información del certificado
        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new KeyInfoX509Data(certificate));
        signedXml.KeyInfo = keyInfo;

        // Configurar algoritmo de firma
        signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;
        signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

        // Firmar
        signedXml.ComputeSignature();

        // Insertar firma en el documento
        var signature = signedXml.GetXml();
        
        // Para DE, insertar después del elemento gTotSub
        // Para eventos, insertar al final del gEve
        if (deNodes.Count > 0)
        {
            var totSubNodes = xmlDoc.GetElementsByTagName("gTotSub");
            if (totSubNodes.Count > 0 && totSubNodes[0]?.ParentNode != null)
            {
                totSubNodes[0].ParentNode.InsertAfter(signature, totSubNodes[0]);
            }
        }
        else
        {
            elementToSign?.AppendChild(signature);
        }

        // Retornar XML firmado
        using var stringWriter = new System.IO.StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace,
            Encoding = System.Text.Encoding.UTF8
        });
        
        xmlDoc.WriteTo(xmlWriter);
        xmlWriter.Flush();
        
        return stringWriter.ToString();
    }
}