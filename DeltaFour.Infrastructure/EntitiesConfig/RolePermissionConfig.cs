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
            builder.Property(rp => rp.RoleId).HasColumnName("role_id").IsRequired();
            builder.Property(rp => rp.ActionId).HasColumnName("action_id").IsRequired();
            builder.Property(rp => rp.LocationId).HasColumnName("location_id").IsRequired();
            builder.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId);
            builder.HasOne(rp => rp.Location).WithMany(l => l.RolePermissions).HasForeignKey(rp => rp.LocationId);
            builder.HasOne(rp => rp.Action).WithMany(a => a.RolePermissions).HasForeignKey(rp => rp.ActionId);
        }
    }
}
