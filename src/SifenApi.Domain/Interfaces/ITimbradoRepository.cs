using SifenApi.Domain.Entities;

namespace SifenApi.Domain.Interfaces;

public interface ITimbradoRepository : IRepository<Timbrado>
{
    Task<Timbrado?> GetVigenteByContribuyenteAsync(Guid contribuyenteId, CancellationToken cancellationToken = default);
    Task<Timbrado?> GetByNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroAsync(string numero, CancellationToken cancellationToken = default);
}