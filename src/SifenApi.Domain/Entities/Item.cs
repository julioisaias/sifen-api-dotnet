using SifenApi.Domain.Common;
using SifenApi.Domain.Enums;
using Ardalis.GuardClauses;

namespace SifenApi.Domain.Entities;

public class Item : BaseEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public string? Observacion { get; private set; }
    public int? PartidaArancelaria { get; private set; }
    public string? Ncm { get; private set; }
    public int UnidadMedida { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal? TipoCambio { get; private set; }
    public decimal? MontoDescuento { get; private set; }
    public decimal? MontoAnticipo { get; private set; }
    public string? Pais { get; private set; }
    public string? PaisDescripcion { get; private set; }
    public TipoIva TipoIva { get; private set; }
    public decimal BaseIva { get; private set; }
    public decimal TasaIva { get; private set; }
    public decimal MontoIva { get; private set; }
    public decimal PrecioTotal { get; private set; }
    
    // Datos adicionales
    public string? Lote { get; private set; }
    public DateTime? Vencimiento { get; private set; }
    public string? NumeroSerie { get; private set; }
    public string? NumeroPedido { get; private set; }
    public string? NumeroSeguimiento { get; private set; }
    
    // Relación con documento
    public Guid DocumentoElectronicoId { get; private set; }
    public DocumentoElectronico DocumentoElectronico { get; private set; } = null!;

    private Item() { }

    public Item(
        string codigo,
        string descripcion,
        int unidadMedida,
        decimal cantidad,
        decimal precioUnitario,
        TipoIva tipoIva,
        decimal tasaIva)
    {
        Codigo = Guard.Against.NullOrWhiteSpace(codigo, nameof(codigo));
        Descripcion = Guard.Against.NullOrWhiteSpace(descripcion, nameof(descripcion));
        UnidadMedida = Guard.Against.NegativeOrZero(unidadMedida, nameof(unidadMedida));
        Cantidad = Guard.Against.NegativeOrZero(cantidad, nameof(cantidad));
        PrecioUnitario = Guard.Against.Negative(precioUnitario, nameof(precioUnitario));
        TipoIva = tipoIva;
        TasaIva = Guard.Against.Negative(tasaIva, nameof(tasaIva));
        
        CalcularTotales();
    }

    public void AplicarDescuento(decimal montoDescuento)
    {
        MontoDescuento = Guard.Against.Negative(montoDescuento, nameof(montoDescuento));
        CalcularTotales();
    }

    private void CalcularTotales()
    {
        var subtotal = Cantidad * PrecioUnitario;
        var totalConDescuento = subtotal - (MontoDescuento ?? 0);
        
        if (TipoIva == TipoIva.Gravado)
        {
            BaseIva = totalConDescuento / (1 + (TasaIva / 100));
            MontoIva = totalConDescuento - BaseIva;
        }
        else
        {
            BaseIva = 0;
            MontoIva = 0;
        }
        
        PrecioTotal = totalConDescuento;
    }
}