using SifenApi.Domain.Entities;

namespace SifenApi.Domain.Interfaces;

public interface IContribuyenteRepository : IRepository<Contribuyente>
{
    Task<Contribuyente?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default);
    Task<Contribuyente?> GetByIdWithEstablecimientosAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteRucAsync(string ruc, CancellationToken cancellationToken = default);
}