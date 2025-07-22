using SifenApi.Domain.Common;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class ActividadEconomica : BaseEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public Guid ContribuyenteId { get; private set; }
    public Contribuyente Contribuyente { get; private set; } = null!;

    private ActividadEconomica() { }

    public ActividadEconomica(string codigo, string descripcion)
    {
        Codigo = Guard.Against.NullOrWhiteSpace(codigo, nameof(codigo));
        Descripcion = Guard.Against.NullOrWhiteSpace(descripcion, nameof(descripcion));
    }
}