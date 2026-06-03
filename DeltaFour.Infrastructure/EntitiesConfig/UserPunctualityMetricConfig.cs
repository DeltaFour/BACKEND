using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class UserPunctualityMetricConfig : IEntityTypeConfiguration<UserPunctualityMetric>
    {
        public void Configure(EntityTypeBuilder<UserPunctualityMetric> builder)
        {
            builder.ToTable("user_punctuality_metrics");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("id");
            builder.Property(m => m.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(m => m.TotalAttendances).HasColumnName("total_attendances").IsRequired();
            builder.Property(m => m.TotalLateAttendances).HasColumnName("total_late_attendances").IsRequired();
            builder.Property(m => m.LatePercentage).HasColumnName("late_percentage").IsRequired();
            builder.Property(m => m.AverageLateMinutes).HasColumnName("average_late_minutes").IsRequired();
            builder.Property(m => m.MaxLateMinutes).HasColumnName("max_late_minutes").IsRequired();
            builder.Property(m => m.TotalAbsences).HasColumnName("total_absences").IsRequired();
            builder.Property(m => m.TotalWorkedDays).HasColumnName("total_worked_days").IsRequired();
            builder.Property(m => m.Cluster).HasColumnName("cluster");
            builder.Property(m => m.LastCalculatedAt).HasColumnName("last_calculated_at").IsRequired();
            builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
            builder.Property(m => m.CreatedAt).HasColumnName("created_at").IsRequired();

            builder.HasOne(m => m.User)
                .WithOne()
                .HasForeignKey<UserPunctualityMetric>(m => m.UserId);

            builder.HasIndex(m => m.UserId).IsUnique();
        }
    }
}
