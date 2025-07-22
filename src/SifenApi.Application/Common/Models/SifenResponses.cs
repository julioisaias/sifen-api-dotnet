namespace SifenApi.Application.Common.Models;

public class SifenResponse
{
    public bool Success { get; set; }
    public string? Codigo { get; set; }
    public string? Mensaje { get; set; }
    public string? NumeroControl { get; set; }
    public string? Protocolo { get; set; }
    public DateTime? FechaRecepcion { get; set; }
    public string? XmlRespuesta { get; set; }
    public List<SifenError>? Errores { get; set; }
}

public class SifenError
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class SifenConsultaResponse : SifenResponse
{
    public string? Estado { get; set; }
    public string? Cdc { get; set; }
    public DateTime? FechaEmision { get; set; }
    public string? RucEmisor { get; set; }
    public string? RazonSocialEmisor { get; set; }
    public decimal? TotalGeneral { get; set; }
}

public class SifenConsultaRucResponse : SifenResponse
{
    public string? Ruc { get; set; }
    public string? RazonSocial { get; set; }
    public string? NombreFantasia { get; set; }
    public string? EstadoContribuyente { get; set; }
    public List<ActividadEconomicaSifen>? ActividadesEconomicas { get; set; }
}

public class ActividadEconomicaSifen
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class SifenConsultaLoteResponse : SifenResponse
{
    public string? NumeroLote { get; set; }
    public string? EstadoLote { get; set; }
    public List<DocumentoLoteResultSifen>? Documentos { get; set; }
}

public class DocumentoLoteResultSifen
{
    public string Cdc { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? MensajeError { get; set; }
}