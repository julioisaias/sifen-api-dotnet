using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;

namespace SifenApi.Domain.Interfaces;

public interface IEventoDocumentoRepository : IRepository<EventoDocumento>
{
    Task<IEnumerable<EventoDocumento>> GetByDocumentoAsync(Guid documentoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventoDocumento>> GetPendientesAsync(int cantidad = 100, CancellationToken cancellationToken = default);
    Task<bool> ExisteEventoAprobadoAsync(Guid documentoId, TipoEvento tipoEvento, CancellationToken cancellationToken = default);
}