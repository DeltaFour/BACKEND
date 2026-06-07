using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnName("type");

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnName("severity");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("title");

        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("message");

        builder.Property(e => e.ReferenceId)
            .HasColumnName("reference_id");

        builder.Property(e => e.IsRead)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_read");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        builder.HasIndex(e => new { e.CompanyId, e.CreatedAt })
            .HasDatabaseName("IX_notifications_company_id_created_at");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
