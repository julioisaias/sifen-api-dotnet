using MediatR;
using SifenApi.Application.DTOs;
using SifenApi.Application.DTOs.Response;

namespace SifenApi.Application.DocumentosElectronicos.Commands;

public class CreateAutoFacturaCommand : IRequest<ApiResponse<DocumentoElectronicoResponse>>
{
    public FacturaDto AutoFactura { get; }
    public Guid ContribuyenteId { get; }
    public string UserId { get; }
    public AutoFacturaDto DatosVendedor { get; }

    public CreateAutoFacturaCommand(
        FacturaDto autoFactura, 
        Guid contribuyenteId, 
        string userId,
        AutoFacturaDto datosVendedor)
    {
        AutoFactura = autoFactura;
        ContribuyenteId = contribuyenteId;
        UserId = userId;
        DatosVendedor = datosVendedor;
    }
}

public class AutoFacturaDto
{
    public int TipoVendedor { get; set; }
    public int DocumentoTipo { get; set; }
    public string DocumentoNumero { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? NumeroCasa { get; set; }
    public int Departamento { get; set; }
    public string DepartamentoDescripcion { get; set; } = string.Empty;
    public int Distrito { get; set; }
    public string DistritoDescripcion { get; set; } = string.Empty;
    public int Ciudad { get; set; }
    public string CiudadDescripcion { get; set; } = string.Empty;
    public TransaccionAutoFacturaDto? Transaccion { get; set; }
}

public class TransaccionAutoFacturaDto
{
    public string Lugar { get; set; } = string.Empty;
    public int Departamento { get; set; }
    public string DepartamentoDescripcion { get; set; } = string.Empty;
    public int Distrito { get; set; }
    public string DistritoDescripcion { get; set; } = string.Empty;
    public int Ciudad { get; set; }
    public string CiudadDescripcion { get; set; } = string.Empty;
}