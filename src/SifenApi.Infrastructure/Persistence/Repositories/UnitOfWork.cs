using Microsoft.EntityFrameworkCore.Storage;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SifenDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    // Repositories
    private IDocumentoElectronicoRepository? _documentosElectronicos;
    private IContribuyenteRepository? _contribuyentes;
    private IClienteRepository? _clientes;
    private ITimbradoRepository? _timbrados;
    private IEventoDocumentoRepository? _eventosDocumentos;

    public UnitOfWork(SifenDbContext context)
    {
        _context = context;
    }

    public IDocumentoElectronicoRepository DocumentosElectronicos =>
        _documentosElectronicos ??= new DocumentoElectronicoRepository(_context);

    public IContribuyenteRepository Contribuyentes =>
        _contribuyentes ??= new ContribuyenteRepository(_context);

    public IClienteRepository Clientes =>
        _clientes ??= new ClienteRepository(_context);

    public ITimbradoRepository Timbrados =>
        _timbrados ??= new TimbradoRepository(_context);

    public IEventoDocumentoRepository EventosDocumentos =>
        _eventosDocumentos ??= new EventoDocumentoRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await (_currentTransaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentTransaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}