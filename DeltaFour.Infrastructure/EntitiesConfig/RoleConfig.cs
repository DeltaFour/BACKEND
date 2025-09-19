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
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(r => r.IsActive)
                .IsRequired();
            builder.Property(r => r.CreatedBy)
                .IsRequired();
            builder.Property(r => r.CreatedAt)
                .IsRequired();
            builder.Property(r => r.UpdatedBy);
            builder.Property(r => r.UpdatedAt);
            builder 
                .HasOne(r => r.Company)
                .WithMany(c => c.Roles)
                .IsRequired()
                .HasForeignKey(r => r.CompanyId);
        }
    }
}
