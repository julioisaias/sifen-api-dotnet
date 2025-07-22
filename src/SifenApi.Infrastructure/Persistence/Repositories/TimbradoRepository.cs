using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence.Repositories;

public class TimbradoRepository : BaseRepository<Timbrado>, ITimbradoRepository
{
    public TimbradoRepository(SifenDbContext context) : base(context)
    {
    }

    public async Task<Timbrado?> GetVigenteByContribuyenteAsync(
        Guid contribuyenteId, 
        CancellationToken cancellationToken = default)
    {
        var fechaActual = DateTime.UtcNow;
        
        return await _dbSet
            .Include(t => t.Contribuyente)
            .Where(t => t.ContribuyenteId == contribuyenteId &&
                       t.FechaInicio <= fechaActual &&
                       t.FechaFin >= fechaActual)
            .OrderByDescending(t => t.FechaInicio)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Timbrado?> GetByNumeroAsync(
        string numero, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Contribuyente)
            .FirstOrDefaultAsync(t => t.Numero == numero, cancellationToken);
    }

    public async Task<bool> ExisteNumeroAsync(
        string numero, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(t => t.Numero == numero, cancellationToken);
    }
}