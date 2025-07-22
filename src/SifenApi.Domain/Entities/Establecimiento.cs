using SifenApi.Domain.Common;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class Establecimiento : BaseEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Denominacion { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public string? NumeroCasa { get; private set; }
    public string? ComplementoDireccion1 { get; private set; }
    public string? ComplementoDireccion2 { get; private set; }
    public int Departamento { get; private set; }
    public string DepartamentoDescripcion { get; private set; } = string.Empty;
    public int Distrito { get; private set; }
    public string DistritoDescripcion { get; private set; } = string.Empty;
    public int Ciudad { get; private set; }
    public string CiudadDescripcion { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public string? Email { get; private set; }
    public Guid ContribuyenteId { get; private set; }
    public Contribuyente Contribuyente { get; private set; } = null!;

    private Establecimiento() { }

    public Establecimiento(
        string codigo,
        string denominacion,
        string direccion,
        int departamento,
        string departamentoDescripcion,
        int distrito,
        string distritoDescripcion,
        int ciudad,
        string ciudadDescripcion)
    {
        Codigo = Guard.Against.NullOrWhiteSpace(codigo, nameof(codigo));
        Denominacion = Guard.Against.NullOrWhiteSpace(denominacion, nameof(denominacion));
        Direccion = Guard.Against.NullOrWhiteSpace(direccion, nameof(direccion));
        Departamento = Guard.Against.NegativeOrZero(departamento, nameof(departamento));
        DepartamentoDescripcion = Guard.Against.NullOrWhiteSpace(departamentoDescripcion, nameof(departamentoDescripcion));
        Distrito = Guard.Against.NegativeOrZero(distrito, nameof(distrito));
        DistritoDescripcion = Guard.Against.NullOrWhiteSpace(distritoDescripcion, nameof(distritoDescripcion));
        Ciudad = Guard.Against.NegativeOrZero(ciudad, nameof(ciudad));
        CiudadDescripcion = Guard.Against.NullOrWhiteSpace(ciudadDescripcion, nameof(ciudadDescripcion));
    }
}