using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class TimeSheetAuditConfig : IEntityTypeConfiguration<TimeSheetAudit>
{
    public void Configure(EntityTypeBuilder<TimeSheetAudit> builder)
    {
        builder.ToTable("time_sheet_audits");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.TimeSheetId)
            .IsRequired()
            .HasColumnName("time_sheet_id");

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(e => e.UserName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("user_name");

        builder.Property(e => e.Operation)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("operation");

        builder.Property(e => e.OldValues)
            .IsRequired()
            .HasColumnType("text")
            .HasColumnName("old_values");

        builder.Property(e => e.NewValues)
            .IsRequired()
            .HasColumnType("text")
            .HasColumnName("new_values");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        builder.HasOne(e => e.TimeSheet)
            .WithMany(t => t.Audits)
            .HasForeignKey(e => e.TimeSheetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
