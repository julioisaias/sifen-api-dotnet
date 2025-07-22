using Microsoft.EntityFrameworkCore;
using SifenApi.Domain.Common;
using SifenApi.Domain.Entities;
using SifenApi.Application.Common.Interfaces;
using System.Reflection;

namespace SifenApi.Infrastructure.Persistence.Context;

public class SifenDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public SifenDbContext(
        DbContextOptions<SifenDbContext> options,
        ICurrentUserService currentUserService,
        IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public DbSet<Contribuyente> Contribuyentes => Set<Contribuyente>();
    public DbSet<ActividadEconomica> ActividadesEconomicas => Set<ActividadEconomica>();
    public DbSet<Establecimiento> Establecimientos => Set<Establecimiento>();
    public DbSet<Timbrado> Timbrados => Set<Timbrado>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<DocumentoElectronico> DocumentosElectronicos => Set<DocumentoElectronico>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<EventoDocumento> EventosDocumentos => Set<EventoDocumento>();
    public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Ignorar DomainEvent ya que es una clase abstracta para eventos del dominio
        modelBuilder.Ignore<DomainEvent>();
        
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedBy(_currentUserService.UserId);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetUpdatedInfo(_currentUserService.UserId);
                    break;
            }
        }

        var events = ChangeTracker.Entries<BaseEntity>()
            .Select(x => x.Entity)
            .Where(x => x is DocumentoElectronico)
            .Cast<DocumentoElectronico>()
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events (could be handled by a domain event dispatcher)
        foreach (var @event in events)
        {
            // TODO: Dispatch event
        }

        return result;
    }
}
