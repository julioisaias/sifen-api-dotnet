namespace SifenApi.Application.DTOs;

public class CondicionDto
{
    public int Tipo { get; set; } = 1; // 1: Contado, 2: Crédito
    public List<EntregaDto>? Entregas { get; set; }
    public CreditoDto? Credito { get; set; }
}

public class EntregaDto
{
    public int Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "PYG";
    public decimal? Cambio { get; set; }
    public InfoTarjetaDto? InfoTarjeta { get; set; }
    public InfoChequeDto? InfoCheque { get; set; }
}

public class InfoTarjetaDto
{
    public int Tipo { get; set; }
    public string? TipoDescripcion { get; set; }
    public string? Titular { get; set; }
    public string? Ruc { get; set; }
    public string? RazonSocial { get; set; }
    public int? MedioPago { get; set; }
    public string? CodigoAutorizacion { get; set; }
}

public class InfoChequeDto
{
    public string NumeroCheque { get; set; } = string.Empty;
    public string Banco { get; set; } = string.Empty;
}

public class CreditoDto
{
    public int Tipo { get; set; } = 1;
    public string? Plazo { get; set; }
    public int? Cuotas { get; set; }
    public decimal? MontoEntrega { get; set; }
    public List<InfoCuotaDto>? InfoCuotas { get; set; }
}

public class InfoCuotaDto
{
    public string Moneda { get; set; } = "PYG";
    public decimal Monto { get; set; }
    public DateTime Vencimiento { get; set; }
}