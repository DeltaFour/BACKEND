using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class CompanyWorkScheduleConfig : IEntityTypeConfiguration<CompanyWorkSchedule>
    {
        public void Configure(EntityTypeBuilder<CompanyWorkSchedule> builder)
        {
            builder.HasKey(cws => cws.Id);
            builder.Property(cws => cws.Id).HasColumnName("id");
            builder.Property(cws => cws.CompanyId).HasColumnName("company_id").IsRequired();
            builder.Property(cws => cws.StartTime).HasColumnName("start_time").IsRequired();
            builder.Property(cws => cws.EndTime).HasColumnName("end_time").IsRequired();
            builder.Property(cws => cws.ToleranceMinutes).HasColumnName("tolerance_minutes").IsRequired();
            builder.Property(cws => cws.UpdatedBy).HasColumnName("updated_by");
            builder.Property(cws => cws.UpdatedAt).HasColumnName("updated_at");
            builder.Property(cws => cws.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(cws => cws.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.HasOne(cws => cws.Company).WithMany(c => c.CompanyWorkSchedules)
                .HasForeignKey(cws => cws.CompanyId).IsRequired();
        }
    }
}
