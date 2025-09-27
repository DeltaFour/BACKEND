using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class EmployeeAuthConfig : IEntityTypeConfiguration<EmployeeAuth>
    {
        public void Configure(EntityTypeBuilder<EmployeeAuth> builder)
        {
            builder.ToTable("employee_auth");
            builder.HasKey(ua => ua.Id);
            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(ua => ua.RefreshToken).HasColumnName("refresh_token").IsRequired();
            builder.Property(ua => ua.ExpiresAt).HasColumnName("expires_at").IsRequired();
            builder.Property(ua => ua.EmployeeId).HasColumnName("employee_id");
            builder.HasOne(au => au.Employee).WithOne(u => u.EmployeeAuth)
                .HasForeignKey<EmployeeAuth>(ua => ua.EmployeeId);
        }

    }
}
