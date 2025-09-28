using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employee");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("name");
        builder.Property(e => e.Email).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("email");
        builder.Property(e => e.Password).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("password");
        builder.Property(e => e.Cellphone).HasMaxLength(14).HasColumnName("cellphone");
        builder.Property(e => e.IsActive).IsRequired().HasColumnName("is_active");
        builder.Property(e => e.IsConfirmed).IsRequired().HasColumnName("is_confirmed");
        builder.Property(e => e.IsAllowedBypassCoord).IsRequired().HasColumnName("is_allowed_by_pass_coord");
        builder.Property(e => e.LastLogin).HasColumnName("last_login");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
        builder.Property(e => e.CompanyId).HasColumnName("company_id").IsRequired();
        builder.Property(e => e.RoleId).HasColumnName("role_id");
        builder.HasOne<Company>(e => e.Company).WithMany(c => c.Employees)
            .HasForeignKey(e => e.CompanyId);
        builder.HasOne<EmployeeAuth>(e => e.EmployeeAuth).WithOne(ea => ea.Employee)
            .HasForeignKey<EmployeeAuth>(ea => ea.EmployeeId);
        builder.HasOne<Role>(e => e.Role).WithOne(r => r.Employee).HasForeignKey<Employee>(e => e.RoleId);
        builder.HasMany(e => e.EmployeeShifts).WithOne(es => es.Employee).HasForeignKey(es => es.EmployeeId);
        builder.HasMany(e => e.EmployeeAttendances).WithOne(ea => ea.Employee)
            .HasForeignKey(ea => ea.EmployeeId);
    }
}
