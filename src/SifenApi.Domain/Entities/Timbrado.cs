using SifenApi.Domain.Common;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class Timbrado : BaseAuditableEntity
{
    public string Numero { get; private set; } = string.Empty;
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public Guid ContribuyenteId { get; private set; }
    public Contribuyente Contribuyente { get; private set; } = null!;
    public List<DocumentoElectronico> DocumentosElectronicos { get; private set; } = new();

    private Timbrado() { }

    public Timbrado(
        string numero,
        DateTime fechaInicio,
        DateTime fechaFin,
        Guid contribuyenteId)
    {
        Numero = Guard.Against.NullOrWhiteSpace(numero, nameof(numero));
        FechaInicio = fechaInicio;
        FechaFin = Guard.Against.InvalidInput(fechaFin, nameof(fechaFin), 
            f => f > fechaInicio, "La fecha fin debe ser mayor a la fecha de inicio");
        ContribuyenteId = contribuyenteId;
    }

    public bool EstaVigente() => DateTime.UtcNow >= FechaInicio && DateTime.UtcNow <= FechaFin;
}