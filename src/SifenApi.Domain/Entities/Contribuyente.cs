using SifenApi.Domain.Common;
using SifenApi.Domain.Enums;
using SifenApi.Domain.ValueObjects;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class Contribuyente : BaseAuditableEntity
{
    public Ruc Ruc { get; private set; } = null!;
    public string RazonSocial { get; private set; } = string.Empty;
    public string? NombreFantasia { get; private set; }
    public TipoContribuyente TipoContribuyente { get; private set; }
    public int TipoRegimen { get; private set; }
    public List<ActividadEconomica> ActividadesEconomicas { get; private set; } = new();
    public List<Establecimiento> Establecimientos { get; private set; } = new();
    public List<Timbrado> Timbrados { get; private set; } = new();
    public List<DocumentoElectronico> DocumentosElectronicos { get; private set; } = new();
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public bool Activo { get; private set; }

    private Contribuyente() { }

    public Contribuyente(
        string ruc,
        string razonSocial,
        TipoContribuyente tipoContribuyente,
        int tipoRegimen)
    {
        Ruc = new Ruc(ruc);
        RazonSocial = Guard.Against.NullOrWhiteSpace(razonSocial, nameof(razonSocial));
        TipoContribuyente = tipoContribuyente;
        TipoRegimen = Guard.Against.NegativeOrZero(tipoRegimen, nameof(tipoRegimen));
        Activo = true;
    }

    public void ActualizarDatos(
        string razonSocial,
        string? nombreFantasia,
        string? email,
        string? telefono)
    {
        RazonSocial = Guard.Against.NullOrWhiteSpace(razonSocial, nameof(razonSocial));
        NombreFantasia = nombreFantasia;
        Email = email;
        Telefono = telefono;
    }

    public void AgregarActividadEconomica(ActividadEconomica actividad)
    {
        Guard.Against.Null(actividad, nameof(actividad));
        ActividadesEconomicas.Add(actividad);
    }

    public void AgregarEstablecimiento(Establecimiento establecimiento)
    {
        Guard.Against.Null(establecimiento, nameof(establecimiento));
        Establecimientos.Add(establecimiento);
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}