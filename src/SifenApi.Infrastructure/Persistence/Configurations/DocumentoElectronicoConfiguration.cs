using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class DocumentoElectronicoConfiguration : IEntityTypeConfiguration<DocumentoElectronico>
{
    public void Configure(EntityTypeBuilder<DocumentoElectronico> builder)
    {
        builder.ToTable("DocumentosElectronicos");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.TipoDocumento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.TipoEmision)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.FechaEmision)
            .IsRequired();

        builder.Property(d => d.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.SubTotal)
            .HasPrecision(18, 2);

        builder.Property(d => d.TotalDescuento)
            .HasPrecision(18, 2);

        builder.Property(d => d.TotalIva)
            .HasPrecision(18, 2);

        builder.Property(d => d.Moneda)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(d => d.CondicionVenta)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.Descripcion)
            .HasMaxLength(1000);

        builder.Property(d => d.Observacion)
            .HasMaxLength(500);

        builder.Property(d => d.MotivoRechazo)
            .HasMaxLength(500);

        builder.Property(d => d.XmlFirmado);

        builder.Property(d => d.QrData)
            .HasMaxLength(500);

        builder.Property(d => d.NumeroControlSifen)
            .HasMaxLength(50);

        builder.Property(d => d.ProtocoloAutorizacion)
            .HasMaxLength(100);

        // Configure CDC value object
        builder.OwnsOne(d => d.Cdc, cdc =>
        {
            cdc.Property(c => c.Value)
                .HasColumnName("Cdc")
                .HasMaxLength(44)
                .IsRequired();

            cdc.HasIndex(c => c.Value)
                .IsUnique();
        });

        // Relationships
        builder.HasOne(d => d.Contribuyente)
            .WithMany(c => c.DocumentosElectronicos)
            .HasForeignKey(d => d.ContribuyenteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Cliente)
            .WithMany(c => c.DocumentosElectronicos)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Timbrado)
            .WithMany()
            .HasForeignKey(d => d.TimbradoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Items)
            .WithOne(i => i.DocumentoElectronico)
            .HasForeignKey(i => i.DocumentoElectronicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Eventos)
            .WithOne(e => e.DocumentoElectronico)
            .HasForeignKey(e => e.DocumentoElectronicoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.TipoDocumento);
        builder.HasIndex(d => d.FechaEmision);
        builder.HasIndex(d => d.Estado);
        builder.HasIndex(d => new { d.ContribuyenteId, d.FechaEmision });

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}