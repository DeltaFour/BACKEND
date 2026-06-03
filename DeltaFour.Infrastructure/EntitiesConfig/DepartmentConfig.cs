using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("department");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("id");
            builder.Property(d => d.Name).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("name");
            builder.Property(d => d.CompanyId).HasColumnName("company_id");
            builder
                .Property(d => d.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow)
                .HasColumnName("created_at");
            builder.Property(d => d.CreatedBy).HasColumnName("created_by");
            builder.Property(d => d.UpdatedAt).HasColumnName("updated_at");
            builder.Property(d => d.UpdatedBy).HasColumnName("updated_by");
            builder.HasOne(d => d.Company).WithMany().HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(d => d.Users).WithOne(u => u.Department).HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
