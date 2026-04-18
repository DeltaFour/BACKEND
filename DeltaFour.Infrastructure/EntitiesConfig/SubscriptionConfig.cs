using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.CompanyId).IsRequired().HasColumnName("company_id");
        builder.Property(s => s.PlanName).IsRequired().HasMaxLength(100).HasColumnName("plan_name");
        builder.Property(s => s.Status).IsRequired().HasMaxLength(50).HasColumnName("status");
        builder.Property(s => s.StartDate).IsRequired().HasColumnName("start_date");
        builder.Property(s => s.EndDate).HasColumnName("end_date");
        builder.Property(s => s.ExternalId).HasMaxLength(255).HasColumnName("external_id");
        builder
            .Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        builder.HasOne(s => s.Company)
            .WithOne(c => c.Subscription)
            .HasForeignKey<Subscription>(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SubscriptionEvents)
            .WithOne(se => se.Subscription)
            .HasForeignKey(se => se.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
