using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Entities;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence.Repositories;

public class ContribuyenteRepository : BaseRepository<Contribuyente>, IContribuyenteRepository
{
    public ContribuyenteRepository(SifenDbContext context) : base(context)
    {
    }

    public async Task<Contribuyente?> GetByRucAsync(
        string ruc, 
        CancellationToken cancellationToken = default)
    {
        // Separar RUC y dígito verificador
        var parts = ruc.Split('-');
        if (parts.Length != 2)
            return null;

        var rucNumero = parts[0].PadLeft(8, '0');
        var digitoVerificador = parts[1];

        return await _dbSet
            .Include(c => c.ActividadesEconomicas)
            .Include(c => c.Establecimientos)
            .FirstOrDefaultAsync(c => 
                c.Ruc.Numero == rucNumero && 
                c.Ruc.DigitoVerificador == digitoVerificador,
                cancellationToken);
    }

    public async Task<Contribuyente?> GetByIdWithEstablecimientosAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ActividadesEconomicas)
            .Include(c => c.Establecimientos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExisteRucAsync(
        string ruc, 
        CancellationToken cancellationToken = default)
    {
        var parts = ruc.Split('-');
        if (parts.Length != 2)
            return false;

        var rucNumero = parts[0].PadLeft(8, '0');
        var digitoVerificador = parts[1];

        return await _dbSet.AnyAsync(c => 
            c.Ruc.Numero == rucNumero && 
            c.Ruc.DigitoVerificador == digitoVerificador,
            cancellationToken);
    }
}
