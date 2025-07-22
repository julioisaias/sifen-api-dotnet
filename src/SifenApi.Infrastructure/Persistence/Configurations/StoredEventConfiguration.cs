using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SifenApi.Infrastructure.Persistence.Configurations;

public class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
{
    public void Configure(EntityTypeBuilder<StoredEvent> builder)
    {
        builder.ToTable("StoredEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.AggregateId)
            .IsRequired();

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Data)
            .IsRequired();

        builder.Property(e => e.OccurredOn)
            .IsRequired();

        builder.HasIndex(e => e.AggregateId);
        builder.HasIndex(e => e.OccurredOn);
        builder.HasIndex(e => new { e.AggregateId, e.EventType });
    }
}