using System.Xml;
using System.Xml.Linq;
using SifenApi.Application.Common.Interfaces;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SifenApi.Infrastructure.Services.Xml;

public class XmlGenerator : IXmlGenerator
{
    private readonly ILogger<XmlGenerator> _logger;
    private const string XmlNamespace = "http://ekuatia.set.gov.py/sifen/xsd";
    private const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

    public XmlGenerator(ILogger<XmlGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<string> GenerateDocumentoElectronicoXmlAsync(
        DocumentoElectronico documento,
        ContribuyenteParams parametros,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                var ns = XNamespace.Get(XmlNamespace);
                var xsi = XNamespace.Get(XsiNamespace);

                var rDE = new XElement(ns + "rDE",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "schemaLocation", 
                        "http://ekuatia.set.gov.py/sifen/xsd siRecepDE_v150.xsd"));

                // Datos inherentes a la operación (A001-A099)
                var gDatGralOpe = new XElement(ns + "gDatGralOpe",
                    new XElement(ns + "dFeEmiDE", documento.FechaEmision.ToString("yyyy-MM-ddTHH:mm:ss"))
                );

                // Campos inherentes a la operación de DE (B001-B099)
                var gOpeDE = new XElement(ns + "gOpeDE",
                    new XElement(ns + "iTipEmi", (int)documento.TipoEmision),
                    new XElement(ns + "dDesTipEmi", GetDescripcionTipoEmision(documento.TipoEmision)),
                    new XElement(ns + "dCodSeg", documento.CodigoSeguridadAleatorio),
                    documento.Observacion != null ? new XElement(ns + "dInfoEmi", documento.Observacion) : null,
                    GetInfoFisc(documento) != null ? new XElement(ns + "dInfoFisc", GetInfoFisc(documento)) : null
                );

                // Campos de datos del Timbrado (C001-C099)
                var gTimb = new XElement(ns + "gTimb",
                    new XElement(ns + "iTiDE", (int)documento.TipoDocumento),
                    new XElement(ns + "dDesTiDE", GetDescripcionTipoDocumento(documento.TipoDocumento)),
                    new XElement(ns + "dNumTim", parametros.TimbradoNumero),
                    new XElement(ns + "dEst", documento.Establecimiento),
                    new XElement(ns + "dPunExp", documento.PuntoExpedicion),
                    new XElement(ns + "dNumDoc", documento.NumeroDocumento),
                    new XElement(ns + "dFeIniT", parametros.TimbradoFecha.ToString("yyyy-MM-dd"))
                );

                // Datos generales inherentes a la operación comercial (D001-D299)
                var gDatGralOpeComercial = new XElement(ns + "gDatGralOpe",
                    new XElement(ns + "iTImp", "1"), // IVA
                    new XElement(ns + "dDesTImp", "IVA"),
                    new XElement(ns + "cMoneOpe", documento.Moneda),
                    documento.Moneda != "PYG" ? new XElement(ns + "dTiCam", documento.TipoCambio) : null
                );

                // Campos que describen al emisor del DE (E001-E099)
                var gEmis = GenerarDatosEmisor(ns, parametros);

                // Campos que identifican al receptor del DE (E201-E399)
                var gDatRec = documento.Cliente != null ? 
                    GenerarDatosReceptor(ns, documento.Cliente) : 
                    GenerarReceptorInnominado(ns);

                // Campos que describen la operación comercial (E600-E799)
                var gDtipDE = GenerarDatosOperacionComercial(ns, documento);

                // Campos que describen los ítems de la operación (E700-E899)
                var gCamItem = new XElement(ns + "gCamItem");
                foreach (var item in documento.Items)
                {
                    gCamItem.Add(GenerarItem(ns, item));
                }

                // Campos de totales de la operación (F001-F099)
                var gTotSub = GenerarTotales(ns, documento);

                // Construir el DE
                var DE = new XElement(ns + "DE",
                    new XAttribute("Id", documento.Cdc?.Value ?? ""),
                    gOpeDE,
                    gTimb,
                    gDatGralOpe,
                    gDatGralOpeComercial,
                    gEmis,
                    gDatRec,
                    gDtipDE,
                    gCamItem,
                    gTotSub
                );

                rDE.Add(DE);

                // Convertir a string con formato
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Encoding = System.Text.Encoding.UTF8
                };

                using var stringWriter = new System.IO.StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, settings);
                rDE.WriteTo(xmlWriter);
                xmlWriter.Flush();
                
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando XML para documento {Cdc}", documento.Cdc?.Value);
                throw;
            }
        }, cancellationToken);
    }

    private XElement GenerarDatosEmisor(XNamespace ns, ContribuyenteParams parametros)
    {
        var gEmis = new XElement(ns + "gEmis",
            new XElement(ns + "dRucEm", parametros.Ruc.Split('-')[0]),
            new XElement(ns + "dDVEmi", parametros.Ruc.Split('-')[1]),
            new XElement(ns + "iTipCont", parametros.TipoContribuyente),
            new XElement(ns + "dNomEmi", parametros.RazonSocial),
            parametros.NombreFantasia != null ? 
                new XElement(ns + "dNomFanEmi", parametros.NombreFantasia) : null,
            new XElement(ns + "iNatEmi", "1"), // Nacional
            new XElement(ns + "iTiOpe", "1") // B2C
        );

        // Actividades económicas
        var gActEco = new XElement(ns + "gActEco");
        foreach (var actividad in parametros.ActividadesEconomicas)
        {
            gActEco.Add(new XElement(ns + "dCodActEco", actividad.Codigo));
        }
        gEmis.Add(gActEco);

        // Establecimientos
        if (parametros.Establecimientos.Any())
        {
            var establecimiento = parametros.Establecimientos.First();
            var gRespDE = new XElement(ns + "gRespDE",
                new XElement(ns + "iTipIDRespDE", "1"), // CI
                new XElement(ns + "dDTipIDRespDE", "Cédula de identidad")
            );
            gEmis.Add(gRespDE);
        }

        return gEmis;
    }

    private XElement? GenerarDatosReceptor(XNamespace ns, Cliente cliente)
    {
        var gDatRec = new XElement(ns + "gDatRec");

        if (cliente.EsContribuyente && cliente.Ruc != null)
        {
            gDatRec.Add(new XElement(ns + "iNatRec", "1")); // Nacional
            gDatRec.Add(new XElement(ns + "iTiOpe", "1")); // B2C
            gDatRec.Add(new XElement(ns + "iTiContRec", "1")); // Contribuyente
            gDatRec.Add(new XElement(ns + "dRucRec", cliente.Ruc.Numero));
            gDatRec.Add(new XElement(ns + "dDVRec", cliente.Ruc.DigitoVerificador));
            gDatRec.Add(new XElement(ns + "dNomRec", cliente.RazonSocial ?? ""));
        }
        else if (!cliente.EsContribuyente)
        {
            gDatRec.Add(new XElement(ns + "iNatRec", "1"));
            gDatRec.Add(new XElement(ns + "iTiOpe", "1"));
            gDatRec.Add(new XElement(ns + "iTiContRec", "2")); // No contribuyente
            gDatRec.Add(new XElement(ns + "iTipIDRec", (int)(cliente.TipoDocumento ?? TipoDocumentoIdentidad.CedulaIdentidad)));
            gDatRec.Add(new XElement(ns + "dDTipIDRec", GetDescripcionTipoDocumento(cliente.TipoDocumento ?? TipoDocumentoIdentidad.CedulaIdentidad)));
            gDatRec.Add(new XElement(ns + "dNumIDRec", cliente.NumeroDocumento ?? ""));
            gDatRec.Add(new XElement(ns + "dNomRec", cliente.Nombre ?? cliente.RazonSocial ?? ""));
        }

        if (!string.IsNullOrEmpty(cliente.Direccion))
        {
            gDatRec.Add(new XElement(ns + "dDirRec", cliente.Direccion));
        }

        if (!string.IsNullOrEmpty(cliente.Telefono))
        {
            gDatRec.Add(new XElement(ns + "dTelRec", cliente.Telefono));
        }

        if (!string.IsNullOrEmpty(cliente.Celular))
        {
            gDatRec.Add(new XElement(ns + "dCelRec", cliente.Celular));
        }

        if (!string.IsNullOrEmpty(cliente.Email))
        {
            gDatRec.Add(new XElement(ns + "dEmailRec", cliente.Email));
        }

        return gDatRec;
    }

    private XElement GenerarReceptorInnominado(XNamespace ns)
    {
        return new XElement(ns + "gDatRec",
            new XElement(ns + "iNatRec", "1"),
            new XElement(ns + "iTiOpe", "1"),
            new XElement(ns + "iTiContRec", "2"),
            new XElement(ns + "iTipIDRec", "5"),
            new XElement(ns + "dDTipIDRec", "Innominado"),
            new XElement(ns + "dNumIDRec", "0"),
            new XElement(ns + "dNomRec", "Sin Nombre")
        );
    }

    private XElement GenerarDatosOperacionComercial(XNamespace ns, DocumentoElectronico documento)
    {
        var gDtipDE = new XElement(ns + "gDtipDE");

        // Para factura electrónica
        if (documento.TipoDocumento == TipoDocumento.FacturaElectronica)
        {
            var gCamFE = new XElement(ns + "gCamFE",
                new XElement(ns + "iIndPres", "1"), // Presencial
                new XElement(ns + "dDesIndPres", "Operación presencial")
            );
            gDtipDE.Add(gCamFE);
        }

        // Condición de la operación
        var gCamCond = new XElement(ns + "gCamCond",
            new XElement(ns + "iCondOpe", (int)documento.CondicionVenta),
            new XElement(ns + "dDCondOpe", documento.CondicionVenta == CondicionVenta.Contado ? "Contado" : "Crédito")
        );

        if (documento.CondicionVenta == CondicionVenta.Contado)
        {
            var gPaConEIni = new XElement(ns + "gPaConEIni",
                new XElement(ns + "iTiPago", "1"),
                new XElement(ns + "dDesTiPag", "Efectivo"),
                new XElement(ns + "dMonTiPag", documento.Total),
                new XElement(ns + "cMoneTiPag", documento.Moneda)
            );
            gCamCond.Add(gPaConEIni);
        }

        gDtipDE.Add(gCamCond);

        return gDtipDE;
    }

    private XElement GenerarItem(XNamespace ns, Item item)
    {
        var gCamItem = new XElement(ns + "gCamItem",
            new XElement(ns + "dCodInt", item.Codigo),
            new XElement(ns + "dDesProSer", item.Descripcion),
            new XElement(ns + "cUniMed", item.UnidadMedida),
            new XElement(ns + "dDesUniMed", GetDescripcionUnidadMedida(item.UnidadMedida)),
            new XElement(ns + "dCantProSer", item.Cantidad),
            new XElement(ns + "gValorItem",
                new XElement(ns + "dPUniProSer", item.PrecioUnitario),
                item.MontoDescuento > 0 ? 
                    new XElement(ns + "gValorRestaItem",
                        new XElement(ns + "dDescItem", item.MontoDescuento),
                        new XElement(ns + "dPorcDesIt", Math.Round((item.MontoDescuento.Value / (item.Cantidad * item.PrecioUnitario)) * 100, 2))
                    ) : null,
                new XElement(ns + "dTotBruOpeItem", item.PrecioTotal)
            ),
            new XElement(ns + "gCamIVA",
                new XElement(ns + "iAfecIVA", (int)item.TipoIva),
                new XElement(ns + "dDesAfecIVA", GetDescripcionTipoIva(item.TipoIva)),
                new XElement(ns + "dPropIVA", "100"),
                new XElement(ns + "dTasaIVA", item.TasaIva),
                new XElement(ns + "dBasGravIVA", item.BaseIva),
                new XElement(ns + "dLiqIVAItem", item.MontoIva)
            )
        );

        return gCamItem;
    }

    private XElement GenerarTotales(XNamespace ns, DocumentoElectronico documento)
    {
        return new XElement(ns + "gTotSub",
            new XElement(ns + "dSubExe", "0"),
            new XElement(ns + "dSubExo", "0"),
            new XElement(ns + "dSub5", documento.Items.Where(i => i.TasaIva == 5).Sum(i => i.BaseIva)),
            new XElement(ns + "dSub10", documento.Items.Where(i => i.TasaIva == 10).Sum(i => i.BaseIva)),
            new XElement(ns + "dTotOpe", documento.SubTotal),
            new XElement(ns + "dTotDesc", documento.TotalDescuento),
            new XElement(ns + "dTotDescGlotem", documento.DescuentoGlobal ?? 0),
            new XElement(ns + "dTotAntItem", documento.AnticipoGlobal ?? 0),
            new XElement(ns + "dTotAnt", documento.AnticipoGlobal ?? 0),
            new XElement(ns + "dPorcDescTotal", documento.SubTotal > 0 ? 
                Math.Round((documento.TotalDescuento / documento.SubTotal) * 100, 2) : 0),
            new XElement(ns + "dDescTotal", documento.TotalDescuento),
            new XElement(ns + "dAnticipo", documento.AnticipoGlobal ?? 0),
            new XElement(ns + "dRedon", "0"),
            new XElement(ns + "dComi", "0"),
            new XElement(ns + "dTotGralOpe", documento.Total),
            new XElement(ns + "dIVA5", documento.Items.Where(i => i.TasaIva == 5).Sum(i => i.MontoIva)),
            new XElement(ns + "dIVA10", documento.Items.Where(i => i.TasaIva == 10).Sum(i => i.MontoIva)),
            new XElement(ns + "dLiqTotIVA5", documento.Items.Where(i => i.TasaIva == 5).Sum(i => i.MontoIva)),
            new XElement(ns + "dLiqTotIVA10", documento.Items.Where(i => i.TasaIva == 10).Sum(i => i.MontoIva)),
            new XElement(ns + "dTotIVA", documento.TotalIva)
        );
    }

    public async Task<string> GenerateEventoXmlAsync(
        string tipoEvento,
        object eventoData,
        ContribuyenteParams parametros,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var ns = XNamespace.Get(XmlNamespace);
            var xsi = XNamespace.Get(XsiNamespace);

            var rEve = new XElement(ns + "rEve",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi));

            // Generar XML según tipo de evento
            switch (tipoEvento.ToLower())
            {
                case "cancelacion":
                    rEve.Add(GenerarEventoCancelacion(ns, eventoData as Dictionary<string, object>));
                    break;
                case "inutilizacion":
                    rEve.Add(GenerarEventoInutilizacion(ns, eventoData as Dictionary<string, object>));
                    break;
                case "conformidad":
                    rEve.Add(GenerarEventoConformidad(ns, eventoData as Dictionary<string, object>));
                    break;
                case "disconformidad":
                    rEve.Add(GenerarEventoDisconformidad(ns, eventoData as Dictionary<string, object>));
                    break;
                case "desconocimiento":
                    rEve.Add(GenerarEventoDesconocimiento(ns, eventoData as Dictionary<string, object>));
                    break;
                case "notificacion":
                    rEve.Add(GenerarEventoNotificacion(ns, eventoData as Dictionary<string, object>));
                    break;
                default:
                    throw new ArgumentException($"Tipo de evento no soportado: {tipoEvento}");
            }

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = System.Text.Encoding.UTF8
            };

            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);
            rEve.WriteTo(xmlWriter);
            xmlWriter.Flush();

            return stringWriter.ToString();
        }, cancellationToken);
    }

    private XElement GenerarEventoCancelacion(XNamespace ns, Dictionary<string, object>? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        return new XElement(ns + "gEve",
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XElement(ns + "dEvReg", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement(ns + "gGroupTiEvt",
                new XElement(ns + "gCamEveCan",
                    new XElement(ns + "dMotCan", data["motivo"]),
                    new XElement(ns + "Id", data["cdc"])
                )
            )
        );
    }

    private XElement GenerarEventoInutilizacion(XNamespace ns, Dictionary<string, object>? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        return new XElement(ns + "gEve",
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XElement(ns + "dEvReg", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement(ns + "gGroupTiEvt",
                new XElement(ns + "gCamEveInu",
                    new XElement(ns + "dMotInu", data["motivo"]),
                    new XElement(ns + "iTiDE", data["tipoDocumento"]),
                    new XElement(ns + "dNumTim", data["timbrado"]),
                    new XElement(ns + "dEst", data["establecimiento"]),
                    new XElement(ns + "dPunExp", data["punto"]),
                    new XElement(ns + "dNumIn", data["desde"]),
                    new XElement(ns + "dNumFin", data["hasta"])
                )
            )
        );
    }

    private XElement GenerarEventoConformidad(XNamespace ns, Dictionary<string, object>? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        return new XElement(ns + "gEve",
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XElement(ns + "dEvReg", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement(ns + "gGroupTiEvt",
                new XElement(ns + "gCamEveCon",
                    new XElement(ns + "iTipConf", data["tipoConformidad"]),
                    new XElement(ns + "dFecRecep", data["fechaRecepcion"]),
                    new XElement(ns + "Id", data["cdc"])
                )
            )
        );
    }

    private XElement GenerarEventoDisconformidad(XNamespace ns, Dictionary<string, object>? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        return new XElement(ns + "gEve",
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XElement(ns + "dEvReg", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement(ns + "gGroupTiEvt",
                new XElement(ns + "gCamEveDis",
                    new XElement(ns + "dMotDis", data["motivo"]),
                    new XElement(ns + "Id", data["cdc"])
                )
            )
        );
    }

    private XElement GenerarEventoDesconocimiento(XNamespace ns, Dictionary<string, object>? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        return new XElement(ns + "gEve",
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XElement(ns + "dEvReg", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement(ns + "gGroupTiEvt",
                new XElement(ns + "gCamEveDes",
                    new XElement(ns + "Id", data["cdc"]),
                    new XElement(ns + "dFecEmi", data["fechaEmision"]),
                    new XElement(ns + "dFecRecep", data["fechaRecepcion"]),
                    new XElement(ns + "iTipRec", data["tipoReceptor"]),
                    new XElement(ns + "dNomRec", data["nombre"]),
                    data.ContainsKey("ruc") ? new XElement(ns + "dRucRec", data["ruc"]) : null,
                    data.ContainsKey("documentoTipo") ? new XElement(ns + "dTipIDRec", data["documentoTipo"]) : null,
                    data.ContainsKey("documentoNumero") ? new XElement(ns + "dNumID", data["documentoNumero"]) : null,
                    new XElement(ns + "dMotDes", data["motivo"])
                )
            )
        );
    }

    private XElement GenerarEventoNotificacion(XNamespace ns, Dictionary<string, object>? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        return new XElement(ns + "gEve",
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XElement(ns + "dEvReg", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XElement(ns + "gGroupTiEvt",
                new XElement(ns + "gCamEveNot",
                    new XElement(ns + "Id", data["cdc"]),
                    new XElement(ns + "dFecEmi", data["fechaEmision"]),
                    new XElement(ns + "dFecRecep", data["fechaRecepcion"]),
                    new XElement(ns + "iTipRec", data["tipoReceptor"]),
                    new XElement(ns + "dNomRec", data["nombre"]),
                    data.ContainsKey("ruc") ? new XElement(ns + "dRucRec", data["ruc"]) : null,
                    data.ContainsKey("documentoTipo") ? new XElement(ns + "dTipIDRec", data["documentoTipo"]) : null,
                    data.ContainsKey("documentoNumero") ? new XElement(ns + "dNumID", data["documentoNumero"]) : null,
                    new XElement(ns + "dTotPYG", data["totalPYG"])
                )
            )
        );
    }

    private string GetDescripcionTipoEmision(TipoEmision tipo)
    {
        return tipo switch
        {
            TipoEmision.Normal => "Normal",
            TipoEmision.Contingencia => "Contingencia",
            _ => "Normal"
        };
    }

    private string GetDescripcionTipoDocumento(TipoDocumento tipo)
    {
        return tipo switch
        {
            TipoDocumento.FacturaElectronica => "Factura electrónica",
            TipoDocumento.FacturaElectronicaExportacion => "Factura electrónica de exportación",
            TipoDocumento.FacturaElectronicaImportacion => "Factura electrónica de importación",
            TipoDocumento.AutofacturaElectronica => "Autofactura electrónica",
            TipoDocumento.NotaCreditoElectronica => "Nota de crédito electrónica",
            TipoDocumento.NotaDebitoElectronica => "Nota de débito electrónica",
            TipoDocumento.NotaRemisionElectronica => "Nota de remisión electrónica",
            TipoDocumento.ComprobanteRetencionElectronico => "Comprobante de retención electrónico",
            _ => "Documento electrónico"
        };
    }

    private string GetDescripcionTipoDocumento(TipoDocumentoIdentidad tipo)
    {
        return tipo switch
        {
            TipoDocumentoIdentidad.CedulaIdentidad => "Cédula de identidad",
            TipoDocumentoIdentidad.Pasaporte => "Pasaporte",
            TipoDocumentoIdentidad.CedulaExtranjera => "Cédula extranjera",
            TipoDocumentoIdentidad.CarnetResidencia => "Carnet de residencia",
            TipoDocumentoIdentidad.Innominado => "Innominado",
            TipoDocumentoIdentidad.TarjetaDiplomaticaConsular => "Tarjeta diplomática consular",
            TipoDocumentoIdentidad.DocumentoDistintivoConvenio => "Documento distintivo convenio",
            _ => "Otro"
        };
    }

    private string GetDescripcionUnidadMedida(int codigo)
    {
        return codigo switch
        {
            77 => "UNIDAD",
            47 => "KILOGRAMO",
            48 => "LITRO",
            10 => "METRO",
            31 => "METRO CUADRADO",
            32 => "METRO CUBICO",
            _ => "UNIDAD"
        };
    }

    private string GetDescripcionTipoIva(TipoIva tipo)
    {
        return tipo switch
        {
            TipoIva.Gravado => "Gravado IVA",
            TipoIva.Exonerado => "Exonerado",
            TipoIva.Exento => "Exento",
            TipoIva.GravadoParcial => "Gravado parcial",
            _ => "Gravado IVA"
        };
    }

    private string? GetInfoFisc(DocumentoElectronico documento)
    {
        if (documento.TipoDocumento == TipoDocumento.NotaRemisionElectronica)
        {
            return "La operación documentada en esta Nota de Remisión Electrónica no genera obligación tributaria de pago de IVA e Impuesto a la Renta por parte del emisor";
        }
        return null;
    }
}
