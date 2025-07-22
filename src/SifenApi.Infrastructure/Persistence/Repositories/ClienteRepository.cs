using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Enums;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence.Repositories;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(SifenDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> GetByRucAsync(
        string ruc, 
        CancellationToken cancellationToken = default)
    {
        var parts = ruc.Split('-');
        if (parts.Length != 2)
            return null;

        var rucNumero = parts[0].PadLeft(8, '0');
        var digitoVerificador = parts[1];

        return await _dbSet
            .FirstOrDefaultAsync(c => 
                c.Ruc != null &&
                c.Ruc.Numero == rucNumero && 
                c.Ruc.DigitoVerificador == digitoVerificador,
                cancellationToken);
    }

    public async Task<Cliente?> GetByDocumentoAsync(
        int tipoDocumento, 
        string numeroDocumento, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => 
                c.TipoDocumento == (TipoDocumentoIdentidad)tipoDocumento &&
                c.NumeroDocumento == numeroDocumento,
                cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> BuscarPorNombreAsync(
        string nombre, 
        CancellationToken cancellationToken = default)
    {
        var nombreLower = nombre.ToLower();
        
        return await _dbSet
            .Where(c => 
                (c.RazonSocial != null && c.RazonSocial.ToLower().Contains(nombreLower)) ||
                (c.NombreFantasia != null && c.NombreFantasia.ToLower().Contains(nombreLower)) ||
                (c.Nombre != null && c.Nombre.ToLower().Contains(nombreLower)))
            .OrderBy(c => c.RazonSocial ?? c.Nombre)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}