using SifenApi.Domain.Common;
using SifenApi.Domain.Enums;
using SifenApi.Domain.ValueObjects;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class Cliente : BaseAuditableEntity
{
    public bool EsContribuyente { get; private set; }
    public Ruc? Ruc { get; private set; }
    public string? RazonSocial { get; private set; }
    public string? NombreFantasia { get; private set; }
    public TipoDocumentoIdentidad? TipoDocumento { get; private set; }
    public string? NumeroDocumento { get; private set; }
    public string? Nombre { get; private set; }
    public string? Direccion { get; private set; }
    public string? NumeroCasa { get; private set; }
    public int? Departamento { get; private set; }
    public string? DepartamentoDescripcion { get; private set; }
    public int? Distrito { get; private set; }
    public string? DistritoDescripcion { get; private set; }
    public int? Ciudad { get; private set; }
    public string? CiudadDescripcion { get; private set; }
    public string? Pais { get; private set; }
    public string? PaisDescripcion { get; private set; }
    public string? Telefono { get; private set; }
    public string? Celular { get; private set; }
    public string? Email { get; private set; }
    public string? Codigo { get; private set; }
    public List<DocumentoElectronico> DocumentosElectronicos { get; private set; } = new();

    private Cliente() { }

    public static Cliente CrearContribuyente(
        string ruc,
        string razonSocial,
        string? direccion = null,
        string? telefono = null,
        string? email = null)
    {
        var cliente = new Cliente
        {
            EsContribuyente = true,
            Ruc = new Ruc(ruc),
            RazonSocial = Guard.Against.NullOrWhiteSpace(razonSocial, nameof(razonSocial)),
            Direccion = direccion,
            Telefono = telefono,
            Email = email
        };

        return cliente;
    }

    public static Cliente CrearNoContribuyente(
        TipoDocumentoIdentidad tipoDocumento,
        string numeroDocumento,
        string nombre,
        string? direccion = null,
        string? telefono = null,
        string? email = null)
    {
        var cliente = new Cliente
        {
            EsContribuyente = false,
            TipoDocumento = tipoDocumento,
            NumeroDocumento = Guard.Against.NullOrWhiteSpace(numeroDocumento, nameof(numeroDocumento)),
            Nombre = Guard.Against.NullOrWhiteSpace(nombre, nameof(nombre)),
            Direccion = direccion,
            Telefono = telefono,
            Email = email
        };

        return cliente;
    }
}