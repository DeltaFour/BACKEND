using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class ClusterCentroidConfig : IEntityTypeConfiguration<ClusterCentroid>
    {
        public void Configure(EntityTypeBuilder<ClusterCentroid> builder)
        {
            builder.ToTable("cluster_centroids");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.CompanyId).HasColumnName("company_id").IsRequired();
            builder.Property(c => c.Cluster).HasColumnName("cluster").IsRequired();
            builder.Property(c => c.LatePercentage).HasColumnName("late_percentage").IsRequired();
            builder.Property(c => c.AverageLateMinutes).HasColumnName("average_late_minutes").IsRequired();
            builder.Property(c => c.MaxLateMinutes).HasColumnName("max_late_minutes").IsRequired();
            builder.Property(c => c.TotalAbsences).HasColumnName("total_absences").IsRequired();
            builder.Property(c => c.TotalWorkedDays).HasColumnName("total_worked_days").IsRequired();
            builder.Property(c => c.CalculatedAt).HasColumnName("calculated_at").IsRequired();
            builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();

            builder.HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId);

            builder.HasIndex(c => new { c.CompanyId, c.Cluster });
        }
    }
}
