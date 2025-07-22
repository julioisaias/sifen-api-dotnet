namespace SifenApi.Application.DTOs;

public class ComplementariosDto
{
    public string? OrdenCompra { get; set; }
    public string? OrdenVenta { get; set; }
    public string? NumeroAsiento { get; set; }
    public CargaDto? Carga { get; set; }
}

public class CargaDto
{
    public string? OrdenCompra { get; set; }
    public string? OrdenVenta { get; set; }
    public string? NumeroAsiento { get; set; }
}
