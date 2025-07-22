using MediatR;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.Consultas.Queries;

public class ConsultarRucSifenQuery : IRequest<ApiResponse<ConsultaRucResponse>>
{
    public string Ruc { get; }

    public ConsultarRucSifenQuery(string ruc)
    {
        Ruc = ruc;
    }
}

public class ConsultaRucResponse
{
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreFantasia { get; set; }
    public string Estado { get; set; } = string.Empty;
    public List<ActividadEconomicaDto> ActividadesEconomicas { get; set; } = new();
}

public class ActividadEconomicaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}