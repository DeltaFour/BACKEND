using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class LocationConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("location");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).HasColumnName("id");
            builder.Property(l => l.Name).IsUnicode(false).HasMaxLength(255).HasColumnName("name").IsRequired();
            builder.HasMany(l => l.RolePermissions).WithOne(rp => rp.Location)
                .HasForeignKey(rl => rl.LocationId);
            builder.HasData(
                new Location{Name = "company"},
                new Location{Name = "employee"},
                new Location{Name = "work"}
            );
        }
    }
}
