using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;
using SifenApi.Domain.ValueObjects;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class ContribuyenteConfiguration : IEntityTypeConfiguration<Contribuyente>
{
    public void Configure(EntityTypeBuilder<Contribuyente> builder)
    {
        builder.ToTable("Contribuyentes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.RazonSocial)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.NombreFantasia)
            .HasMaxLength(255);

        builder.Property(c => c.TipoContribuyente)
            .IsRequired();

        builder.Property(c => c.TipoRegimen)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(100);

        builder.Property(c => c.Telefono)
            .HasMaxLength(50);

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Value Object conversion
        builder.OwnsOne(c => c.Ruc, ruc =>
        {
            ruc.Property(r => r.Numero)
                .HasColumnName("RucNumero")
                .HasMaxLength(8)
                .IsRequired();

            ruc.Property(r => r.DigitoVerificador)
                .HasColumnName("RucDigitoVerificador")
                .HasMaxLength(1)
                .IsRequired();

            ruc.HasIndex(r => new { r.Numero, r.DigitoVerificador })
                .IsUnique();
        });

        builder.HasMany(c => c.ActividadesEconomicas)
            .WithOne(a => a.Contribuyente)
            .HasForeignKey(a => a.ContribuyenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Establecimientos)
            .WithOne(e => e.Contribuyente)
            .HasForeignKey(e => e.ContribuyenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Timbrados)
            .WithOne(t => t.Contribuyente)
            .HasForeignKey(t => t.ContribuyenteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.DocumentosElectronicos)
            .WithOne(d => d.Contribuyente)
            .HasForeignKey(d => d.ContribuyenteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit fields
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(100);
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100);
        builder.Property(c => c.IsDeleted).HasDefaultValue(false);
        builder.Property(c => c.DeletedAt);
        builder.Property(c => c.DeletedBy).HasMaxLength(100);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}