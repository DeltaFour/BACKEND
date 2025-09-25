using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permission");
            builder.HasKey(rp => rp.Id);
            builder.Property(rp => rp.Id).HasColumnName("id");
        }
    }
}
