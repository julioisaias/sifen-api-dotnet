namespace SifenApi.Application.DTOs;

public class SectorAdicionalDto
{
    public string? Ciclo { get; set; }
    public DateTime? InicioCiclo { get; set; }
    public DateTime? FinCiclo { get; set; }
    public DateTime? VencimientoPago { get; set; }
    public string? NumeroContrato { get; set; }
    public decimal? SaldoAnterior { get; set; }
}