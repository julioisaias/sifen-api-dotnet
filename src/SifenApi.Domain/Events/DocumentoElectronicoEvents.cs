using SifenApi.Domain.Common;
using SifenApi.Domain.Entities;

namespace SifenApi.Domain.Events;

public class DocumentoElectronicoCreatedEvent : DomainEvent
{
    public DocumentoElectronico DocumentoElectronico { get; }

    public DocumentoElectronicoCreatedEvent(DocumentoElectronico documentoElectronico)
    {
        DocumentoElectronico = documentoElectronico;
    }
}

public class DocumentoElectronicoApprovedEvent : DomainEvent
{
    public DocumentoElectronico DocumentoElectronico { get; }

    public DocumentoElectronicoApprovedEvent(DocumentoElectronico documentoElectronico)
    {
        DocumentoElectronico = documentoElectronico;
    }
}

public class DocumentoElectronicoRejectedEvent : DomainEvent
{
    public DocumentoElectronico DocumentoElectronico { get; }
    public string Motivo { get; }

    public DocumentoElectronicoRejectedEvent(DocumentoElectronico documentoElectronico, string motivo)
    {
        DocumentoElectronico = documentoElectronico;
        Motivo = motivo;
    }
}

public class DocumentoElectronicoCanceledEvent : DomainEvent
{
    public DocumentoElectronico DocumentoElectronico { get; }
    public string Motivo { get; }

    public DocumentoElectronicoCanceledEvent(DocumentoElectronico documentoElectronico, string motivo)
    {
        DocumentoElectronico = documentoElectronico;
        Motivo = motivo;
    }
}
