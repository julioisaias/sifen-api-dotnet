namespace SifenApi.Application.DTOs;

public class ClienteDto
{
    public bool Contribuyente { get; set; }
    public string? Ruc { get; set; }
    public string? RazonSocial { get; set; }
    public string? NombreFantasia { get; set; }
    public int TipoOperacion { get; set; } = 1;
    public string? Direccion { get; set; }
    public string? NumeroCasa { get; set; }
    public int? Departamento { get; set; }
    public string? DepartamentoDescripcion { get; set; }
    public int? Distrito { get; set; }
    public string? DistritoDescripcion { get; set; }
    public int? Ciudad { get; set; }
    public string? CiudadDescripcion { get; set; }
    public string? Pais { get; set; } = "PRY";
    public string? PaisDescripcion { get; set; } = "Paraguay";
    public int? TipoContribuyente { get; set; }
    public int? DocumentoTipo { get; set; }
    public string? DocumentoNumero { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }
    public string? Codigo { get; set; }
}