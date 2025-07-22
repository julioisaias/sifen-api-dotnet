using SifenApi.Domain.Common;
using SifenApi.Domain.Enums;
using SifenApi.Domain.ValueObjects;
using SifenApi.Domain.Events;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class DocumentoElectronico : BaseAuditableEntity
{
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Datos del documento
    public Cdc? Cdc { get; private set; }
    public TipoDocumento TipoDocumento { get; private set; }
    public string Establecimiento { get; private set; } = string.Empty;
    public string PuntoExpedicion { get; private set; } = string.Empty;
    public string NumeroDocumento { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public string? Observacion { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public TipoEmision TipoEmision { get; private set; }
    public TipoTransaccion TipoTransaccion { get; private set; }
    public string CodigoSeguridadAleatorio { get; private set; } = string.Empty;
    
    // Datos del timbrado
    public Guid TimbradoId { get; private set; }
    public Timbrado Timbrado { get; private set; } = null!;
    
    // Datos del emisor
    public Guid ContribuyenteId { get; private set; }
    public Contribuyente Contribuyente { get; private set; } = null!;
    
    // Datos del receptor
    public Guid? ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }
    
    // Datos de la operación
    public string Moneda { get; private set; } = "PYG";
    public decimal? TipoCambio { get; private set; }
    public CondicionVenta CondicionVenta { get; private set; }
    public decimal? DescuentoGlobal { get; private set; }
    public decimal? AnticipoGlobal { get; private set; }
    
    // Items
    public List<Item> Items { get; private set; } = new();
    
    // Totales
    public decimal SubTotal { get; private set; }
    public decimal TotalDescuento { get; private set; }
    public decimal TotalIva { get; private set; }
    public decimal Total { get; private set; }
    
    // Estado del documento
    public EstadoDocumento Estado { get; private set; }
    public string? MotivoRechazo { get; private set; }
    
    // Datos técnicos
    public string? XmlFirmado { get; private set; }
    public string? QrData { get; private set; }
    public string? NumeroControlSifen { get; private set; }
    public DateTime? FechaAprobacion { get; private set; }
    public string? ProtocoloAutorizacion { get; private set; }
    
    // Eventos
    public List<EventoDocumento> Eventos { get; private set; } = new();

    private DocumentoElectronico() { }

    public DocumentoElectronico(
        TipoDocumento tipoDocumento,
        string establecimiento,
        string puntoExpedicion,
        string numeroDocumento,
        DateTime fechaEmision,
        TipoEmision tipoEmision,
        TipoTransaccion tipoTransaccion,
        Guid timbradoId,
        Guid contribuyenteId,
        string moneda,
        CondicionVenta condicionVenta)
    {
        TipoDocumento = tipoDocumento;
        Establecimiento = Guard.Against.NullOrWhiteSpace(establecimiento, nameof(establecimiento));
        PuntoExpedicion = Guard.Against.NullOrWhiteSpace(puntoExpedicion, nameof(puntoExpedicion));
        NumeroDocumento = Guard.Against.NullOrWhiteSpace(numeroDocumento, nameof(numeroDocumento));
        FechaEmision = fechaEmision;
        TipoEmision = tipoEmision;
        TipoTransaccion = tipoTransaccion;
        TimbradoId = timbradoId;
        ContribuyenteId = contribuyenteId;
        Moneda = Guard.Against.NullOrWhiteSpace(moneda, nameof(moneda));
        CondicionVenta = condicionVenta;
        Estado = EstadoDocumento.Pendiente;
        
        GenerarCodigoSeguridad();
        
        AddDomainEvent(new DocumentoElectronicoCreatedEvent(this));
    }

    public void AsignarCliente(Cliente cliente)
    {
        Cliente = Guard.Against.Null(cliente, nameof(cliente));
        ClienteId = cliente.Id;
    }

    public void AgregarItem(Item item)
    {
        Guard.Against.Null(item, nameof(item));
        Items.Add(item);
        CalcularTotales();
    }

    public void GenerarCdc(Contribuyente contribuyente)
    {
        Guard.Against.Null(contribuyente, nameof(contribuyente));
        
        Cdc = Cdc.Generate(
            ((int)TipoDocumento).ToString(),
            contribuyente.Ruc.Numero,
            contribuyente.Ruc.DigitoVerificador,
            Establecimiento,
            PuntoExpedicion,
            NumeroDocumento,
            ((int)contribuyente.TipoContribuyente).ToString(),
            FechaEmision,
            ((int)TipoEmision).ToString(),
            CodigoSeguridadAleatorio
        );
    }

    public void EstablecerXmlFirmado(string xmlFirmado)
    {
        XmlFirmado = Guard.Against.NullOrWhiteSpace(xmlFirmado, nameof(xmlFirmado));
        Estado = EstadoDocumento.EnProceso;
    }

    public void EstablecerQr(string qrData)
    {
        QrData = Guard.Against.NullOrWhiteSpace(qrData, nameof(qrData));
    }

    public void AprobarDocumento(string numeroControl, string protocolo)
    {
        NumeroControlSifen = Guard.Against.NullOrWhiteSpace(numeroControl, nameof(numeroControl));
        ProtocoloAutorizacion = Guard.Against.NullOrWhiteSpace(protocolo, nameof(protocolo));
        FechaAprobacion = DateTime.UtcNow;
        Estado = EstadoDocumento.Aprobado;
        
        AddDomainEvent(new DocumentoElectronicoApprovedEvent(this));
    }

    public void RechazarDocumento(string motivo)
    {
        MotivoRechazo = Guard.Against.NullOrWhiteSpace(motivo, nameof(motivo));
        Estado = EstadoDocumento.Rechazado;
        
        AddDomainEvent(new DocumentoElectronicoRejectedEvent(this, motivo));
    }

    public void CancelarDocumento(string motivo, string usuario)
    {
        Guard.Against.InvalidInput(Estado, nameof(Estado), 
            e => e == EstadoDocumento.Aprobado, 
            "Solo se pueden cancelar documentos aprobados");
        
        Estado = EstadoDocumento.Cancelado;
        var evento = new EventoDocumento(
            TipoEvento.Cancelacion,
            motivo,
            Id,
            usuario
        );
        
        Eventos.Add(evento);
        
        AddDomainEvent(new DocumentoElectronicoCanceledEvent(this, motivo));
    }

    private void CalcularTotales()
    {
        SubTotal = Items.Sum(i => i.PrecioTotal);
        TotalDescuento = Items.Sum(i => i.MontoDescuento ?? 0) + (DescuentoGlobal ?? 0);
        TotalIva = Items.Sum(i => i.MontoIva);
        Total = SubTotal - TotalDescuento + TotalIva;
    }

    private void GenerarCodigoSeguridad()
    {
        var random = new Random();
        CodigoSeguridadAleatorio = random.Next(100000000, 999999999).ToString();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}