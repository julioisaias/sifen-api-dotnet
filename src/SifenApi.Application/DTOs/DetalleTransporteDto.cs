namespace SifenApi.Application.DTOs;

public class DetalleTransporteDto
{
    public int Tipo { get; set; }
    public int Modalidad { get; set; }
    public int TipoResponsable { get; set; }
    public string? CondicionNegociacion { get; set; }
    public string? NumeroManifiesto { get; set; }
    public string? NumeroDespachoImportacion { get; set; }
    public DateTime? InicioEstimadoTranslado { get; set; }
    public DateTime? FinEstimadoTranslado { get; set; }
    public string? PaisDestino { get; set; }
    public string? PaisDestinoNombre { get; set; }
    public DireccionTransporteDto? Salida { get; set; }
    public DireccionTransporteDto? Entrega { get; set; }
    public VehiculoDto? Vehiculo { get; set; }
    public TransportistaDto? Transportista { get; set; }
}

public class DireccionTransporteDto
{
    public string Direccion { get; set; } = string.Empty;
    public string? NumeroCasa { get; set; }
    public string? ComplementoDireccion1 { get; set; }
    public string? ComplementoDireccion2 { get; set; }
    public int? Departamento { get; set; }
    public string? DepartamentoDescripcion { get; set; }
    public int? Distrito { get; set; }
    public string? DistritoDescripcion { get; set; }
    public int? Ciudad { get; set; }
    public string? CiudadDescripcion { get; set; }
    public string? Pais { get; set; }
    public string? PaisDescripcion { get; set; }
    public string? TelefonoContacto { get; set; }
}

public class VehiculoDto
{
    public int Tipo { get; set; }
    public string? Marca { get; set; }
    public int? DocumentoTipo { get; set; }
    public string? DocumentoNumero { get; set; }
    public string? Obs { get; set; }
    public string? NumeroMatricula { get; set; }
    public string? NumeroVuelo { get; set; }
}

public class TransportistaDto
{
    public bool Contribuyente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Ruc { get; set; }
    public int? DocumentoTipo { get; set; }
    public string? DocumentoNumero { get; set; }
    public string? Direccion { get; set; }
    public string? Obs { get; set; }
    public string? Pais { get; set; }
    public string? PaisDescripcion { get; set; }
    public ChoferDto? Chofer { get; set; }
    public AgenteDto? Agente { get; set; }
}

public class ChoferDto
{
    public string DocumentoNumero { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
}

public class AgenteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Ruc { get; set; }
    public string? Direccion { get; set; }
}