using SifenApi.Domain.Entities;

namespace SifenApi.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByDocumentoAsync(int tipoDocumento, string numeroDocumento, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken = default);
}