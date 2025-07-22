using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class ActividadEconomicaConfiguration : IEntityTypeConfiguration<ActividadEconomica>
{
    public void Configure(EntityTypeBuilder<ActividadEconomica> builder)
    {
        builder.ToTable("ActividadesEconomicas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Codigo)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.Descripcion)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(a => a.Contribuyente)
            .WithMany(c => c.ActividadesEconomicas)
            .HasForeignKey(a => a.ContribuyenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.ContribuyenteId, a.Codigo })
            .IsUnique();
    }
}