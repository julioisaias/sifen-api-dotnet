using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence.Repositories;

public class DocumentoElectronicoRepository : BaseRepository<DocumentoElectronico>, IDocumentoElectronicoRepository
{
    public DocumentoElectronicoRepository(SifenDbContext context) : base(context)
    {
    }

    public async Task<DocumentoElectronico?> GetByIdWithDetailsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Contribuyente)
                .ThenInclude(c => c.ActividadesEconomicas)
            .Include(d => d.Contribuyente)
                .ThenInclude(c => c.Establecimientos)
            .Include(d => d.Timbrado)
            .Include(d => d.Cliente)
            .Include(d => d.Items)
            .Include(d => d.Eventos)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DocumentoElectronico?> GetByCdcAsync(
        string cdc, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Contribuyente)
            .Include(d => d.Cliente)
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Cdc != null && d.Cdc.Value == cdc, cancellationToken);
    }

    public async Task<IEnumerable<DocumentoElectronico>> GetByContribuyenteAsync(
        Guid contribuyenteId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Cliente)
            .Where(d => d.ContribuyenteId == contribuyenteId)
            .OrderByDescending(d => d.FechaEmision)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentoElectronico>> GetByEstadoAsync(
        EstadoDocumento estado, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Contribuyente)
            .Include(d => d.Cliente)
            .Where(d => d.Estado == estado)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentoElectronico>> GetPendientesEnvioAsync(
        int cantidad = 100, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Contribuyente)
                .ThenInclude(c => c.ActividadesEconomicas)
            .Include(d => d.Contribuyente)
                .ThenInclude(c => c.Establecimientos)
            .Include(d => d.Timbrado)
            .Include(d => d.Cliente)
            .Include(d => d.Items)
            .Where(d => d.Estado == EstadoDocumento.Pendiente && 
                       d.TipoEmision == TipoEmision.Normal &&
                       d.XmlFirmado != null)
            .OrderBy(d => d.CreatedAt)
            .Take(cantidad)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteNumeroDocumentoAsync(
        string establecimiento, 
        string puntoExpedicion, 
        string numeroDocumento, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(d => 
            d.Establecimiento == establecimiento &&
            d.PuntoExpedicion == puntoExpedicion &&
            d.NumeroDocumento == numeroDocumento,
            cancellationToken);
    }

    public async Task<string> GetProximoNumeroAsync(
        string establecimiento, 
        string puntoExpedicion, 
        TipoDocumento tipoDocumento, 
        CancellationToken cancellationToken = default)
    {
        var ultimoNumero = await _dbSet
            .Where(d => d.Establecimiento == establecimiento &&
                       d.PuntoExpedicion == puntoExpedicion &&
                       d.TipoDocumento == tipoDocumento)
            .Select(d => d.NumeroDocumento)
            .OrderByDescending(n => n)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(ultimoNumero))
            return "0000001";

        if (int.TryParse(ultimoNumero, out int numero))
        {
            numero++;
            return numero.ToString("D7");
        }

        return "0000001";
    }
}