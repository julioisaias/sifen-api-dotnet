using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SifenApi.Application.Common.Interfaces;

namespace SifenApi.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClient _smtpClient;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUser = _configuration["Email:SmtpUser"] ?? "";
        var smtpPassword = _configuration["Email:SmtpPassword"] ?? "";

        _smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPassword),
            EnableSsl = true
        };
    }

    public async Task SendDocumentoElectronicoAsync(
        string to,
        string subject,
        string cdc,
        byte[] kudePdf,
        string? xmlContent = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@sifen.com.py";
            var fromName = _configuration["Email:FromName"] ?? "Sistema SIFEN";

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                IsBodyHtml = true
            };

            message.To.Add(to);

            // Cuerpo del email
            var body = $@"
                <html>
                <body>
                    <h2>Documento Electrónico</h2>
                    <p>Se adjunta el documento electrónico correspondiente.</p>
                    <p><strong>CDC:</strong> {cdc}</p>
                    <p>Este es un documento electrónico válido según las normativas de la SET.</p>
                    <br/>
                    <p>Atentamente,<br/>{fromName}</p>
                </body>
                </html>";

            message.Body = body;

            // Adjuntar KUDE PDF
            var kudeAttachment = new Attachment(new MemoryStream(kudePdf), $"KUDE_{cdc}.pdf", "application/pdf");
            message.Attachments.Add(kudeAttachment);

            // Adjuntar XML si se proporciona
            if (!string.IsNullOrEmpty(xmlContent))
            {
                var xmlBytes = System.Text.Encoding.UTF8.GetBytes(xmlContent);
                var xmlAttachment = new Attachment(new MemoryStream(xmlBytes), $"DE_{cdc}.xml", "application/xml");
                message.Attachments.Add(xmlAttachment);
            }

            await _smtpClient.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Email enviado exitosamente a {To} con CDC {Cdc}", to, cdc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email a {To}", to);
            throw;
        }
    }

    public async Task SendNotificationAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@sifen.com.py";
            var fromName = _configuration["Email:FromName"] ?? "Sistema SIFEN";

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await _smtpClient.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Notificación enviada exitosamente a {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando notificación a {To}", to);
            throw;
        }
    }
}