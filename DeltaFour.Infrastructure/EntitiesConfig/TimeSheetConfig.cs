using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class TimeSheetConfig : IEntityTypeConfiguration<TimeSheet>
{
    public void Configure(EntityTypeBuilder<TimeSheet> builder)
    {
        builder.ToTable("time_sheet");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(e => e.Month)
            .IsRequired()
            .HasColumnName("month");

        builder.Property(e => e.Year)
            .IsRequired()
            .HasColumnName("year");

        builder.Property(e => e.SignedByEmployee)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("signed_by_employee");

        builder.Property(e => e.EmployeeSignedAt)
            .HasColumnName("employee_signed_at");

        builder.Property(e => e.SignedByHR)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("signed_by_hr");

        builder.Property(e => e.HRSignedAt)
            .HasColumnName("hr_signed_at");

        builder.Property(e => e.SignedByHRUserId)
            .HasColumnName("signed_by_hr_user_id");

        builder.Property(e => e.SignedByHRUserName)
            .HasMaxLength(255)
            .HasColumnName("signed_by_hr_user_name");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        // Índice único para garantir apenas um registro por usuário/mês/ano
        builder.HasIndex(e => new { e.UserId, e.Month, e.Year })
            .IsUnique()
            .HasDatabaseName("IX_time_sheet_user_month_year");

        // Relacionamento com User
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
