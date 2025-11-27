using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class WorkShiftConfig : IEntityTypeConfiguration<WorkShift>
    {
        public void Configure(EntityTypeBuilder<WorkShift> builder)
        {
            builder.ToTable("work_shift");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(ws => ws.ShiftType).HasConversion(
                v => v.ToString(), v => (ShiftType)Enum.Parse(typeof(ShiftType), v) 
            ).HasColumnName("shift_type").IsRequired();
            builder.Property(ws => ws.StartTime).HasColumnName("starter_time").IsRequired();
            builder.Property(ws => ws.EndTime).HasColumnName("end_time").IsRequired();
            builder.Property(ws => ws.ToleranceMinutes).HasColumnName("tolerance_minutes").IsRequired();
            builder.Property(ws => ws.CompanyId).HasColumnName("company_id").IsRequired();
            builder.Property(ws => ws.UpdatedAt).HasColumnName("updated_at");
            builder.Property(ws => ws.UpdatedBy).HasColumnName("updated_by");
            builder.Property(ws => ws.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(ws => ws.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.HasMany(ws => ws.UserShifts).WithOne(es => es.WorkShift).HasForeignKey(es => es.ShiftId);
            builder.HasOne(ws => ws.Company).WithMany(c => c.WorkShifts).HasForeignKey(ws => ws.CompanyId);
        }
    }
}
