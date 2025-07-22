namespace SifenApi.Application.DTOs;

public class DocumentoAsociadoDto
{
    public int Formato { get; set; }
    public string? Cdc { get; set; }
    public int? Tipo { get; set; }
    public string? Timbrado { get; set; }
    public string? Establecimiento { get; set; }
    public string? Punto { get; set; }
    public string? Numero { get; set; }
    public DateTime? Fecha { get; set; }
    public string? NumeroRetencion { get; set; }
    public string? ResolucionCreditoFiscal { get; set; }
    public int? ConstanciaTipo { get; set; }
    public int? ConstanciaNumero { get; set; }
    public string? ConstanciaControl { get; set; }
}