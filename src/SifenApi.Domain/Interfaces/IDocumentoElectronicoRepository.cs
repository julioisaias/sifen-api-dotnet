using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;

namespace SifenApi.Domain.Interfaces;

public interface IDocumentoElectronicoRepository : IRepository<DocumentoElectronico>
{
    Task<DocumentoElectronico?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DocumentoElectronico?> GetByCdcAsync(string cdc, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentoElectronico>> GetByContribuyenteAsync(Guid contribuyenteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentoElectronico>> GetByEstadoAsync(EstadoDocumento estado, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentoElectronico>> GetPendientesEnvioAsync(int cantidad = 100, CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroDocumentoAsync(string establecimiento, string puntoExpedicion, string numeroDocumento, CancellationToken cancellationToken = default);
    Task<string> GetProximoNumeroAsync(string establecimiento, string puntoExpedicion, TipoDocumento tipoDocumento, CancellationToken cancellationToken = default);
}