using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("role");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("name")
                .IsRequired();
            builder.Property(r => r.IsActive).IsRequired().HasColumnName("is_active");
            builder.Property(r => r.CreatedBy).IsRequired().HasColumnName("created_by");
            builder.Property(r => r.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(r => r.UpdatedBy).HasColumnName("updated_by");
            builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            builder.Property(r => r.CompanyId).HasColumnName("company_id").IsRequired();
            builder.HasOne(r => r.Company).WithMany(c => c.Roles).IsRequired()
                .HasForeignKey(r => r.CompanyId);
            builder.HasMany(r => r.RolePermissions).WithOne(rp => rp.Role).HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
