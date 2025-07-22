namespace SifenApi.Application.DTOs.Request;

public class CreateFacturaSimpleRequest
{
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public ClienteSimpleDto Cliente { get; set; } = new();
    public List<ItemSimpleDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class ClienteSimpleDto
{
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

public class ItemSimpleDto
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal? Descuento { get; set; }
}