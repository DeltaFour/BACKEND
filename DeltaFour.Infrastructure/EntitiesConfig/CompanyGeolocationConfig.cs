using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class CompanyGeolocationConfig : IEntityTypeConfiguration<CompanyGeolocation>
    {
        public void Configure(EntityTypeBuilder<CompanyGeolocation> builder)
        {
            builder.ToTable("company_geolocation");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Coordinates).HasColumnName("coordinates");
            builder.Property(x => x.RadiusMeters).HasColumnName("radius_meters");
            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at");
            builder.HasOne(cg => cg.Company).WithOne(c => c.CompanyGeolocation)
                .HasForeignKey<CompanyGeolocation>(x => x.CompanyId)
                .IsRequired();
            builder.Property(x => x.CompanyId).HasColumnName("company_id").IsRequired();
        }
    }
}
