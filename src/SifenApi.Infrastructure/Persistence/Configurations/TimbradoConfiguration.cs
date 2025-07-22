using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class TimbradoConfiguration : IEntityTypeConfiguration<Timbrado>
{
    public void Configure(EntityTypeBuilder<Timbrado> builder)
    {
        builder.ToTable("Timbrados");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Numero)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(t => t.FechaInicio)
            .IsRequired();

        builder.Property(t => t.FechaFin)
            .IsRequired();

        builder.HasOne(t => t.Contribuyente)
            .WithMany(c => c.Timbrados)
            .HasForeignKey(t => t.ContribuyenteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.DocumentosElectronicos)
            .WithOne(d => d.Timbrado)
            .HasForeignKey(d => d.TimbradoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.Numero)
            .IsUnique();

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}