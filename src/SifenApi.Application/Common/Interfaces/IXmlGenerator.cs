using SifenApi.Domain.Entities;

namespace SifenApi.Application.Common.Interfaces;

public interface IXmlGenerator
{
    Task<string> GenerateDocumentoElectronicoXmlAsync(
        DocumentoElectronico documento, 
        ContribuyenteParams parametros,
        CancellationToken cancellationToken = default);
    
    Task<string> GenerateEventoXmlAsync(
        string tipoEvento, 
        object eventoData, 
        ContribuyenteParams parametros,
        CancellationToken cancellationToken = default);
}

public class ContribuyenteParams
{
    public string Version { get; set; } = "150";
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreFantasia { get; set; }
    public List<ActividadEconomicaParams> ActividadesEconomicas { get; set; } = new();
    public string TimbradoNumero { get; set; } = string.Empty;
    public DateTime TimbradoFecha { get; set; }
    public int TipoContribuyente { get; set; }
    public int TipoRegimen { get; set; }
    public List<EstablecimientoParams> Establecimientos { get; set; } = new();
}

public class ActividadEconomicaParams
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class EstablecimientoParams
{
    public string Codigo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? NumeroCasa { get; set; }
    public string? ComplementoDireccion1 { get; set; }
    public string? ComplementoDireccion2 { get; set; }
    public int Departamento { get; set; }
    public string DepartamentoDescripcion { get; set; } = string.Empty;
    public int Distrito { get; set; }
    public string DistritoDescripcion { get; set; } = string.Empty;
    public int Ciudad { get; set; }
    public string CiudadDescripcion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string Denominacion { get; set; } = string.Empty;
}
