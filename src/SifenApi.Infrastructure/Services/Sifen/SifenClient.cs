using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Application.Common.Models;
using SifenApi.Infrastructure.Security;

namespace SifenApi.Infrastructure.Services.Sifen;

public class SifenClient : ISifenClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SifenClient> _logger;
    private readonly CertificateManager _certificateManager;

    private readonly string _urlTest;
    private readonly string _urlProd;

    public SifenClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<SifenClient> logger,
        CertificateManager certificateManager)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _certificateManager = certificateManager;

        _urlTest = _configuration["Sifen:UrlTest"] ?? "https://sifen-test.set.gov.py/de/ws/sync";
        _urlProd = _configuration["Sifen:UrlProd"] ?? "https://sifen.set.gov.py/de/ws/sync";
    }

    public async Task<SifenResponse> RecibeAsync(
        string xmlDocumento,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ambiente == "prod" ? _urlProd : _urlTest;
            var endpoint = $"{url}/recibe";

            // Configurar certificado cliente si es necesario
            ConfigurarCertificadoCliente(certificatePath, certificatePassword);

            // Crear SOAP envelope
            var soapEnvelope = CreateSoapEnvelope("rEnviDE", xmlDocumento);

            // Crear request
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
            };

            // Headers
            request.Headers.Add("SOAPAction", "recibe");

            // Enviar request
            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Respuesta SIFEN: {Response}", responseContent);

            // Parsear respuesta
            return ParseSifenResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando documento a SIFEN");
            return new SifenResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = $"Error de comunicación: {ex.Message}"
            };
        }
    }

    public async Task<SifenResponse> RecibeLoteAsync(
        List<string> xmlDocumentos,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ambiente == "prod" ? _urlProd : _urlTest;
            var endpoint = $"{url}/recibe-lote";

            // Configurar certificado cliente
            ConfigurarCertificadoCliente(certificatePath, certificatePassword);

            // Crear XML de lote
            var loteXml = CreateLoteXml(xmlDocumentos);

            // Crear SOAP envelope
            var soapEnvelope = CreateSoapEnvelope("rEnviDeLote", loteXml);

            // Crear request
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
            };

            request.Headers.Add("SOAPAction", "recibe-lote");

            // Enviar request
            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Parsear respuesta
            return ParseSifenResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando lote a SIFEN");
            return new SifenResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = $"Error de comunicación: {ex.Message}"
            };
        }
    }

    public async Task<SifenResponse> EventoAsync(
        string xmlEvento,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ambiente == "prod" ? _urlProd : _urlTest;
            var endpoint = $"{url}/evento";

            ConfigurarCertificadoCliente(certificatePath, certificatePassword);

            var soapEnvelope = CreateSoapEnvelope("rEnviEventoDE", xmlEvento);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
            };

            request.Headers.Add("SOAPAction", "evento");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return ParseSifenResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando evento a SIFEN");
            return new SifenResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = $"Error de comunicación: {ex.Message}"
            };
        }
    }

    public async Task<SifenConsultaResponse> ConsultaAsync(
        string cdc,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ambiente == "prod" ? _urlProd : _urlTest;
            var endpoint = $"{url}/consulta";

            ConfigurarCertificadoCliente(certificatePath, certificatePassword);

            // Crear XML de consulta
            var consultaXml = $@"<rEnviConsDe>
                <dId>{Guid.NewGuid()}</dId>
                <dCDC>{cdc}</dCDC>
            </rEnviConsDe>";

            var soapEnvelope = CreateSoapEnvelope("rEnviConsDe", consultaXml);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
            };

            request.Headers.Add("SOAPAction", "consulta");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return ParseConsultaResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando documento en SIFEN");
            return new SifenConsultaResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = $"Error de comunicación: {ex.Message}"
            };
        }
    }

    public async Task<SifenConsultaRucResponse> ConsultaRucAsync(
        string ruc,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ambiente == "prod" ? _urlProd : _urlTest;
            var endpoint = $"{url}/consulta-ruc";

            ConfigurarCertificadoCliente(certificatePath, certificatePassword);

            // Crear XML de consulta RUC
            var consultaXml = $@"<rEnviConsRUC>
                <dId>{Guid.NewGuid()}</dId>
                <dRUCCons>{ruc}</dRUCCons>
            </rEnviConsRUC>";

            var soapEnvelope = CreateSoapEnvelope("rEnviConsRUC", consultaXml);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
            };

            request.Headers.Add("SOAPAction", "consulta-ruc");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return ParseConsultaRucResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando RUC en SIFEN");
            return new SifenConsultaRucResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = $"Error de comunicación: {ex.Message}"
            };
        }
    }

    public async Task<SifenConsultaLoteResponse> ConsultaLoteAsync(
        string numeroLote,
        string ambiente,
        string certificatePath,
        string certificatePassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ambiente == "prod" ? _urlProd : _urlTest;
            var endpoint = $"{url}/consulta-lote";

            ConfigurarCertificadoCliente(certificatePath, certificatePassword);

            var consultaXml = $@"<rEnviConsLoteDe>
                <dId>{Guid.NewGuid()}</dId>
                <dNumLote>{numeroLote}</dNumLote>
            </rEnviConsLoteDe>";

            var soapEnvelope = CreateSoapEnvelope("rEnviConsLoteDe", consultaXml);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
            };

            request.Headers.Add("SOAPAction", "consulta-lote");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return ParseConsultaLoteResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando lote en SIFEN");
            return new SifenConsultaLoteResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = $"Error de comunicación: {ex.Message}"
            };
        }
    }

    private void ConfigurarCertificadoCliente(string certificatePath, string certificatePassword)
    {
        // En producción, configurar el certificado cliente para autenticación mutua TLS
        // Por ahora, solo log
        _logger.LogDebug("Configurando certificado cliente desde {Path}", certificatePath);
    }

    private string CreateSoapEnvelope(string operation, string xmlContent)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" 
               xmlns:ns=""http://ekuatia.set.gov.py/sifen/xsd"">
    <soap:Header/>
    <soap:Body>
        <ns:{operation}>
            {xmlContent}
        </ns:{operation}>
    </soap:Body>
</soap:Envelope>";
    }

    private string CreateLoteXml(List<string> xmlDocumentos)
    {
        var loteId = Guid.NewGuid().ToString();
        var loteXml = new StringBuilder();
        
        loteXml.AppendLine($"<rLoteDE>");
        loteXml.AppendLine($"<dId>{loteId}</dId>");
        
        foreach (var xml in xmlDocumentos)
        {
            // Extraer solo el contenido del DE
            var doc = XDocument.Parse(xml);
            var deElement = doc.Descendants("DE").FirstOrDefault();
            if (deElement != null)
            {
                loteXml.AppendLine(deElement.ToString());
            }
        }
        
        loteXml.AppendLine("</rLoteDE>");
        
        return loteXml.ToString();
    }

    private SifenResponse ParseSifenResponse(string responseXml)
    {
        try
        {
            var doc = XDocument.Parse(responseXml);
            var ns = XNamespace.Get("http://ekuatia.set.gov.py/sifen/xsd");
            
            // Buscar elementos de respuesta
            var estadoElement = doc.Descendants(ns + "dEstRes").FirstOrDefault();
            var codigoElement = doc.Descendants(ns + "dCodRes").FirstOrDefault();
            var mensajeElement = doc.Descendants(ns + "dMsgRes").FirstOrDefault();
            var protocoloElement = doc.Descendants(ns + "dProtAut").FirstOrDefault();
            var numeroControlElement = doc.Descendants(ns + "Id").FirstOrDefault();

            var estado = estadoElement?.Value ?? "";
            var codigo = codigoElement?.Value ?? "";
            
            return new SifenResponse
            {
                Success = estado == "Aprobado",
                Codigo = codigo,
                Mensaje = mensajeElement?.Value,
                Protocolo = protocoloElement?.Value,
                NumeroControl = numeroControlElement?.Value,
                FechaRecepcion = DateTime.UtcNow,
                XmlRespuesta = responseXml
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parseando respuesta SIFEN");
            return new SifenResponse
            {
                Success = false,
                Codigo = "9999",
                Mensaje = "Error parseando respuesta"
            };
        }
    }

    private SifenConsultaResponse ParseConsultaResponse(string responseXml)
    {
        var baseResponse = ParseSifenResponse(responseXml);
        
        // TODO: Parsear campos específicos de consulta
        return new SifenConsultaResponse
        {
            Success = baseResponse.Success,
            Codigo = baseResponse.Codigo,
            Mensaje = baseResponse.Mensaje,
            XmlRespuesta = responseXml
        };
    }

    private SifenConsultaRucResponse ParseConsultaRucResponse(string responseXml)
    {
        var baseResponse = ParseSifenResponse(responseXml);
        
        // TODO: Parsear campos específicos de consulta RUC
        return new SifenConsultaRucResponse
        {
            Success = baseResponse.Success,
            Codigo = baseResponse.Codigo,
            Mensaje = baseResponse.Mensaje,
            XmlRespuesta = responseXml
        };
    }

    private SifenConsultaLoteResponse ParseConsultaLoteResponse(string responseXml)
    {
        var baseResponse = ParseSifenResponse(responseXml);
        
        // TODO: Parsear campos específicos de consulta lote
        return new SifenConsultaLoteResponse
        {
            Success = baseResponse.Success,
            Codigo = baseResponse.Codigo,
            Mensaje = baseResponse.Mensaje,
            XmlRespuesta = responseXml
        };
    }
}