namespace SifenApi.Domain.Enums;

public enum TipoDocumentoIdentidad
{
    CedulaIdentidad = 1,
    Pasaporte = 2,
    CedulaExtranjera = 3,
    CarnetResidencia = 4,
    Innominado = 5,
    TarjetaDiplomaticaConsular = 6,
    DocumentoDistintivoConvenio = 9
}

public enum TipoDocumento
{
    FacturaElectronica = 1,
    FacturaElectronicaExportacion = 2,
    FacturaElectronicaImportacion = 3,
    AutofacturaElectronica = 4,
    NotaCreditoElectronica = 5,
    NotaDebitoElectronica = 6,
    NotaRemisionElectronica = 7,
    ComprobanteRetencionElectronico = 8
}