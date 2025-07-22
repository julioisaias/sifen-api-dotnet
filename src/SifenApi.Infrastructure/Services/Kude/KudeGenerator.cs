using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font;
using System.Xml.Linq;
using System.Text;
using SifenApi.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace SifenApi.Infrastructure.Services.Kude;

public class KudeGenerator : IKudeGenerator
{
    private readonly IQrGenerator _qrGenerator;
    private readonly ILogger<KudeGenerator> _logger;

    public KudeGenerator(IQrGenerator qrGenerator, ILogger<KudeGenerator> logger)
    {
        _qrGenerator = qrGenerator;
        _logger = logger;
    }

    public async Task<byte[]> GenerateKudePdfAsync(
        string xmlDocumento,
        string qrData,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = new PdfWriter(memoryStream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Parsear XML
                var doc = XDocument.Parse(xmlDocumento);
                var ns = XNamespace.Get("http://ekuatia.set.gov.py/sifen/xsd");

                // Configurar página
                document.SetMargins(20, 20, 20, 20);

                // Agregar título
                var title = new Paragraph("KUDE - DOCUMENTO ELECTRÓNICO")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16)
                    .SetBold();
                document.Add(title);

                // Información del documento
                var cdc = doc.Descendants(ns + "DE").FirstOrDefault()?.Attribute("Id")?.Value ?? "";
                var tipoDoc = doc.Descendants(ns + "dDesTiDE").FirstOrDefault()?.Value ?? "";
                var numero = $"{doc.Descendants(ns + "dEst").FirstOrDefault()?.Value}-" +
                            $"{doc.Descendants(ns + "dPunExp").FirstOrDefault()?.Value}-" +
                            $"{doc.Descendants(ns + "dNumDoc").FirstOrDefault()?.Value}";

                document.Add(new Paragraph($"CDC: {cdc}").SetFontSize(10));
                document.Add(new Paragraph($"Tipo: {tipoDoc}").SetFontSize(10));
                document.Add(new Paragraph($"Número: {numero}").SetFontSize(10));

                // Datos del emisor
                document.Add(new Paragraph("DATOS DEL EMISOR").SetBold().SetMarginTop(10));
                var rucEmisor = doc.Descendants(ns + "dRucEm").FirstOrDefault()?.Value ?? "";
                var dvEmisor = doc.Descendants(ns + "dDVEmi").FirstOrDefault()?.Value ?? "";
                var nombreEmisor = doc.Descendants(ns + "dNomEmi").FirstOrDefault()?.Value ?? "";
                
                document.Add(new Paragraph($"RUC: {rucEmisor}-{dvEmisor}"));
                document.Add(new Paragraph($"Razón Social: {nombreEmisor}"));

                // Datos del receptor
                document.Add(new Paragraph("DATOS DEL RECEPTOR").SetBold().SetMarginTop(10));
                var nombreReceptor = doc.Descendants(ns + "dNomRec").FirstOrDefault()?.Value ?? "Innominado";
                var rucReceptor = doc.Descendants(ns + "dRucRec").FirstOrDefault()?.Value;
                var dvReceptor = doc.Descendants(ns + "dDVRec").FirstOrDefault()?.Value;

                document.Add(new Paragraph($"Nombre: {nombreReceptor}"));
                if (!string.IsNullOrEmpty(rucReceptor))
                {
                    document.Add(new Paragraph($"RUC: {rucReceptor}-{dvReceptor}"));
                }

                // Items
                document.Add(new Paragraph("DETALLE").SetBold().SetMarginTop(10));
                
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 40, 15, 15, 20 }))
                    .UseAllAvailableWidth();
                
                // Headers
                table.AddHeaderCell(new Cell().Add(new Paragraph("Código").SetBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Descripción").SetBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad").SetBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Precio Unit.").SetBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetBold()));

                // Items
                var items = doc.Descendants(ns + "gCamItem");
                foreach (var item in items)
                {
                    var codigo = item.Element(ns + "dCodInt")?.Value ?? "";
                    var descripcion = item.Element(ns + "dDesProSer")?.Value ?? "";
                    var cantidad = item.Element(ns + "dCantProSer")?.Value ?? "0";
                    var precioUnit = item.Element(ns + "dPUniProSer")?.Value ?? "0";
                    var total = item.Descendants(ns + "dTotBruOpeItem").FirstOrDefault()?.Value ?? "0";

                    table.AddCell(new Cell().Add(new Paragraph(codigo)));
                    table.AddCell(new Cell().Add(new Paragraph(descripcion)));
                    table.AddCell(new Cell().Add(new Paragraph(cantidad).SetTextAlignment(TextAlignment.RIGHT)));
                    table.AddCell(new Cell().Add(new Paragraph(FormatNumber(precioUnit)).SetTextAlignment(TextAlignment.RIGHT)));
                    table.AddCell(new Cell().Add(new Paragraph(FormatNumber(total)).SetTextAlignment(TextAlignment.RIGHT)));
                }

                document.Add(table);

                // Totales
                document.Add(new Paragraph("TOTALES").SetBold().SetMarginTop(10));
                var totalGeneral = doc.Descendants(ns + "dTotGralOpe").FirstOrDefault()?.Value ?? "0";
                var totalIva = doc.Descendants(ns + "dTotIVA").FirstOrDefault()?.Value ?? "0";
                
                document.Add(new Paragraph($"Total IVA: {FormatNumber(totalIva)}").SetTextAlignment(TextAlignment.RIGHT));
                document.Add(new Paragraph($"TOTAL GENERAL: {FormatNumber(totalGeneral)}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBold()
                    .SetFontSize(14));

                // QR Code
                var qrImage = _qrGenerator.GenerateQrImage(qrData);
                var imageData = ImageDataFactory.Create(qrImage);
                var qrImg = new Image(imageData)
                    .SetWidth(100)
                    .SetHeight(100)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                
                document.Add(new Paragraph().Add(qrImg).SetMarginTop(20));

                // Mensaje final
                document.Add(new Paragraph("Este documento es una representación gráfica del Documento Electrónico")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(8)
                    .SetMarginTop(10));

                document.Close();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando KUDE PDF");
                throw;
            }
        }, cancellationToken);
    }

    public async Task<string> GenerateKudeHtmlAsync(
        string xmlDocumento,
        string qrData,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                var doc = XDocument.Parse(xmlDocumento);
                var ns = XNamespace.Get("http://ekuatia.set.gov.py/sifen/xsd");

                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset='UTF-8'>");
                html.AppendLine("<title>KUDE - Documento Electrónico</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { text-align: center; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin: 10px 0; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; }");
                html.AppendLine(".text-right { text-align: right; }");
                html.AppendLine(".text-center { text-align: center; }");
                html.AppendLine(".bold { font-weight: bold; }");
                html.AppendLine(".qr-code { text-align: center; margin: 20px 0; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                // Contenido similar al PDF pero en HTML
                html.AppendLine("<h1>KUDE - DOCUMENTO ELECTRÓNICO</h1>");

                // Información básica
                var cdc = doc.Descendants(ns + "DE").FirstOrDefault()?.Attribute("Id")?.Value ?? "";
                html.AppendLine($"<p><strong>CDC:</strong> {cdc}</p>");

                // ... continuar con el resto del contenido ...

                html.AppendLine("</body>");
                html.AppendLine("</html>");

                return html.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando KUDE HTML");
                throw;
            }
        }, cancellationToken);
    }

    private string FormatNumber(string number)
    {
        if (decimal.TryParse(number, out var value))
        {
            return value.ToString("N0");
        }
        return number;
    }
}