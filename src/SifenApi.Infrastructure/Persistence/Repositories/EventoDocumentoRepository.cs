using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence.Repositories;

public class EventoDocumentoRepository : BaseRepository<EventoDocumento>, IEventoDocumentoRepository
{
    public EventoDocumentoRepository(SifenDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EventoDocumento>> GetByDocumentoAsync(
        Guid documentoId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.DocumentoElectronicoId == documentoId)
            .OrderByDescending(e => e.FechaEvento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EventoDocumento>> GetPendientesAsync(
        int cantidad = 100, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.DocumentoElectronico)
                .ThenInclude(d => d.Contribuyente)
            .Where(e => e.Estado == EstadoEvento.Pendiente || e.Estado == EstadoEvento.Enviado)
            .OrderBy(e => e.CreatedAt)
            .Take(cantidad)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteEventoAprobadoAsync(
        Guid documentoId, 
        TipoEvento tipoEvento, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => 
            e.DocumentoElectronicoId == documentoId &&
            e.TipoEvento == tipoEvento &&
            e.Estado == EstadoEvento.Aprobado,
            cancellationToken);
    }
}
