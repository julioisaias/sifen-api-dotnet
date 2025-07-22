using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class EstablecimientoConfiguration : IEntityTypeConfiguration<Establecimiento>
{
    public void Configure(EntityTypeBuilder<Establecimiento> builder)
    {
        builder.ToTable("Establecimientos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Codigo)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(e => e.Denominacion)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Direccion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.NumeroCasa)
            .HasMaxLength(10);

        builder.Property(e => e.ComplementoDireccion1)
            .HasMaxLength(255);

        builder.Property(e => e.ComplementoDireccion2)
            .HasMaxLength(255);

        builder.Property(e => e.Departamento)
            .IsRequired();

        builder.Property(e => e.DepartamentoDescripcion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Distrito)
            .IsRequired();

        builder.Property(e => e.DistritoDescripcion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Ciudad)
            .IsRequired();

        builder.Property(e => e.CiudadDescripcion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Telefono)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.HasOne(e => e.Contribuyente)
            .WithMany(c => c.Establecimientos)
            .HasForeignKey(e => e.ContribuyenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ContribuyenteId, e.Codigo })
            .IsUnique();
    }
}