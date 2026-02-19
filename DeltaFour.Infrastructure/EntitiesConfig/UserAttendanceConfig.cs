using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Coordinates = DeltaFour.Domain.Entities.Coordinates;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class UserAttendanceConfig : IEntityTypeConfiguration<UserAttendance>
    {
        public void Configure(EntityTypeBuilder<UserAttendance> builder)
        {
            builder.ToTable("user_attendance");
            builder.HasKey(ea => ea.Id);
            builder.Property(ea => ea.Id).HasColumnName("id");
            builder.Property(ea => ea.UserId).HasColumnName("employee_id").IsRequired();
            builder.Property(ea => ea.PunchTime).HasColumnName("punch_time").IsRequired();
            builder.Property(ea => ea.PunchType) .HasConversion(
                v => v.ToString(), v => (PunchType)Enum.Parse(typeof(PunchType), v)
            ).HasColumnName("punch_type").IsRequired();
            builder.Property(ea => ea.ShiftType).HasColumnName("shift_type").IsRequired();
            builder.Property(ea => ea.Coord).HasConversion(
                    v => new Point(v.Longitude, v.Latitude) { SRID = 4326 },
                    v => new Coordinates(v.Y, v.X)).IsRequired().HasColumnType("point")
                .HasColumnName("coord");
            builder.Property(ea => ea.IsLate).HasColumnName("is_late");
            builder.Property(ea => ea.TimeLate).HasColumnName("time_late");
            builder.Property(ea => ea.UpdatedAt).HasColumnName("updated_at");
            builder.Property(ea => ea.UpdatedBy).HasColumnName("updated_by");
            builder.Property(ea => ea.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(ea => ea.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.HasOne<User>(ea => ea.User).WithMany(e => e.UserAttendances)
                .HasForeignKey(ea => ea.UserId);
        }
    }
}
