namespace SifenApi.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDocumentoElectronicoRepository DocumentosElectronicos { get; }
    IContribuyenteRepository Contribuyentes { get; }
    IClienteRepository Clientes { get; }
    ITimbradoRepository Timbrados { get; }
    IEventoDocumentoRepository EventosDocumentos { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}