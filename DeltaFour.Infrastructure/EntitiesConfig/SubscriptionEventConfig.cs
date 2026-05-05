using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class SubscriptionEventConfig : IEntityTypeConfiguration<SubscriptionEvent>
{
    public void Configure(EntityTypeBuilder<SubscriptionEvent> builder)
    {
        builder.ToTable("subscription_events");
        builder.HasKey(se => se.Id);
        builder.Property(se => se.Id).HasColumnName("id");
        builder.Property(se => se.SubscriptionId).IsRequired().HasColumnName("subscription_id");
        builder.Property(se => se.EventType).IsRequired().HasMaxLength(100).HasColumnName("event_type");
        builder.Property(se => se.Payload).IsRequired().HasColumnType("text").HasColumnName("payload");
        builder
            .Property(se => se.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        builder.HasOne(se => se.Subscription)
            .WithMany(s => s.SubscriptionEvents)
            .HasForeignKey(se => se.SubscriptionId);
    }
}
