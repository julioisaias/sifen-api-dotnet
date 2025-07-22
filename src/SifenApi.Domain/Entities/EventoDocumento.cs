using SifenApi.Domain.Common;
using SifenApi.Domain.Enums;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class EventoDocumento : BaseEntity
{
    public TipoEvento TipoEvento { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public DateTime FechaEvento { get; private set; }
    public EstadoEvento Estado { get; private set; }
    public string? Respuesta { get; private set; }
    public string? XmlEvento { get; private set; }
    public string? XmlRespuesta { get; private set; }
    public Guid DocumentoElectronicoId { get; private set; }
    public DocumentoElectronico DocumentoElectronico { get; private set; } = null!;
    public string UsuarioId { get; private set; } = string.Empty;

    private EventoDocumento() { }

    public EventoDocumento(
        TipoEvento tipoEvento,
        string motivo,
        Guid documentoElectronicoId,
        string usuarioId)
    {
        TipoEvento = tipoEvento;
        Motivo = Guard.Against.NullOrWhiteSpace(motivo, nameof(motivo));
        DocumentoElectronicoId = documentoElectronicoId;
        UsuarioId = Guard.Against.NullOrWhiteSpace(usuarioId, nameof(usuarioId));
        FechaEvento = DateTime.UtcNow;
        Estado = EstadoEvento.Pendiente;
    }

    public void EstablecerXmlEvento(string xmlEvento)
    {
        XmlEvento = Guard.Against.NullOrWhiteSpace(xmlEvento, nameof(xmlEvento));
        Estado = EstadoEvento.Enviado;
    }

    public void EstablecerRespuesta(string respuesta, string xmlRespuesta, bool aprobado)
    {
        Respuesta = respuesta;
        XmlRespuesta = xmlRespuesta;
        Estado = aprobado ? EstadoEvento.Aprobado : EstadoEvento.Rechazado;
    }
}