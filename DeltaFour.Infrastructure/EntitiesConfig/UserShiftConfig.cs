using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class UserShiftConfig : IEntityTypeConfiguration<UserShift>
    {
        public void Configure(EntityTypeBuilder<UserShift> builder)
        {
            builder.ToTable("user_shift");
            builder.Property(es => es.EmployeeId).HasColumnName("employee_id").IsRequired();
            builder.Property(es => es.ShiftId).HasColumnName("shift_id").IsRequired();
            builder.Property(es => es.StartDate).HasColumnName("start_date").IsRequired();
            builder.Property(es => es.EndDate).HasColumnName("end_date");
            builder.Property(es => es.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(false);
            builder.Property(es => es.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(es => es.UpdatedAt).HasColumnName("updated_at");
            builder.Property(es => es.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(es => es.UpdatedBy).HasColumnName("updated_by");
            builder.HasOne<User>(es => es.Employee).WithMany(e => e.EmployeeShifts)
                .HasForeignKey(es => es.EmployeeId);
            builder.HasOne<WorkShift>(es => es.WorkShift).WithMany(e => e.EmployeeShifts)
                .HasForeignKey(es => es.ShiftId);
        }
    }
}
