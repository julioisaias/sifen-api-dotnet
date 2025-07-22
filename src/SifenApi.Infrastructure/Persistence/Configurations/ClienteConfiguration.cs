using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.EsContribuyente)
            .IsRequired();

        builder.Property(c => c.RazonSocial)
            .HasMaxLength(255);

        builder.Property(c => c.NombreFantasia)
            .HasMaxLength(255);

        builder.Property(c => c.NumeroDocumento)
            .HasMaxLength(20);

        builder.Property(c => c.Nombre)
            .HasMaxLength(255);

        builder.Property(c => c.Direccion)
            .HasMaxLength(500);

        builder.Property(c => c.NumeroCasa)
            .HasMaxLength(10);

        builder.Property(c => c.DepartamentoDescripcion)
            .HasMaxLength(100);

        builder.Property(c => c.DistritoDescripcion)
            .HasMaxLength(100);

        builder.Property(c => c.CiudadDescripcion)
            .HasMaxLength(100);

        builder.Property(c => c.Pais)
            .HasMaxLength(3);

        builder.Property(c => c.PaisDescripcion)
            .HasMaxLength(100);

        builder.Property(c => c.Telefono)
            .HasMaxLength(50);

        builder.Property(c => c.Celular)
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(100);

        builder.Property(c => c.Codigo)
            .HasMaxLength(50);

        builder.OwnsOne(c => c.Ruc, ruc =>
        {
            ruc.Property(r => r.Numero)
                .HasColumnName("RucNumero")
                .HasMaxLength(8);

            ruc.Property(r => r.DigitoVerificador)
                .HasColumnName("RucDigitoVerificador")
                .HasMaxLength(1);

            ruc.HasIndex(r => new { r.Numero, r.DigitoVerificador });
        });

        builder.HasMany(c => c.DocumentosElectronicos)
            .WithOne(d => d.Cliente)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.TipoDocumento, c.NumeroDocumento })
            .HasFilter("TipoDocumento IS NOT NULL AND NumeroDocumento IS NOT NULL");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}