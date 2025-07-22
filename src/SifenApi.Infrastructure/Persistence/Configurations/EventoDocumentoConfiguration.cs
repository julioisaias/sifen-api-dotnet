using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SifenApi.Domain.Entities;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class EventoDocumentoConfiguration : IEntityTypeConfiguration<EventoDocumento>
{
    public void Configure(EntityTypeBuilder<EventoDocumento> builder)
    {
        builder.ToTable("EventosDocumentos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TipoEvento)
            .IsRequired();

        builder.Property(e => e.Motivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FechaEvento)
            .IsRequired();

        builder.Property(e => e.Estado)
            .IsRequired();

        builder.Property(e => e.Respuesta)
            .HasMaxLength(500);

        builder.Property(e => e.XmlEvento)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.XmlRespuesta)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.UsuarioId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(e => e.DocumentoElectronico)
            .WithMany(d => d.Eventos)
            .HasForeignKey(e => e.DocumentoElectronicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.DocumentoElectronicoId);
        builder.HasIndex(e => e.Estado);
        builder.HasIndex(e => e.TipoEvento);
    }
}