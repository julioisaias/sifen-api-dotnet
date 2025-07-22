using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Observacion)
            .HasMaxLength(500);

        builder.Property(i => i.PartidaArancelaria);

        builder.Property(i => i.Ncm)
            .HasMaxLength(8);

        builder.Property(i => i.UnidadMedida)
            .IsRequired();

        builder.Property(i => i.Cantidad)
            .HasColumnType("decimal(18,3)")
            .IsRequired();

        builder.Property(i => i.PrecioUnitario)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.TipoCambio)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.MontoDescuento)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.MontoAnticipo)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Pais)
            .HasMaxLength(3);

        builder.Property(i => i.PaisDescripcion)
            .HasMaxLength(100);

        builder.Property(i => i.TipoIva)
            .IsRequired();

        builder.Property(i => i.BaseIva)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.TasaIva)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(i => i.MontoIva)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.PrecioTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.Lote)
            .HasMaxLength(50);

        builder.Property(i => i.Vencimiento);

        builder.Property(i => i.NumeroSerie)
            .HasMaxLength(50);

        builder.Property(i => i.NumeroPedido)
            .HasMaxLength(50);

        builder.Property(i => i.NumeroSeguimiento)
            .HasMaxLength(50);

        builder.HasOne(i => i.DocumentoElectronico)
            .WithMany(d => d.Items)
            .HasForeignKey(i => i.DocumentoElectronicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
