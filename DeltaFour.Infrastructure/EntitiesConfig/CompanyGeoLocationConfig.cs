using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Coordinates = DeltaFour.Domain.Entities.Coordinates;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class CompanyGeoLocationConfig : IEntityTypeConfiguration<CompanyGeolocation>
    {
        public void Configure(EntityTypeBuilder<CompanyGeolocation> builder)
        {
            builder.ToTable("company_geolocation");
            builder.HasKey(cgl => cgl.Id);
            builder.Property(cgl => cgl.Id).HasColumnName("id");
            builder.Property(cgl => cgl.Coord).HasConversion(
                v => new Point(v.Longitude, v.Latitude) { SRID = 3857 }, 
                v => new Coordinates(v.Y, v.X)
            ).HasColumnType("point").HasColumnName("coord").IsRequired();
            builder.Property(cgl => cgl.RadiusMeters).HasColumnName("radius_meters").IsRequired();
            builder.Property(cgl => cgl.IsActive).HasColumnName("is_active").IsRequired();
            builder.Property(cgl => cgl.UpdatedAt).HasColumnName("updated_at");
            builder.Property(cgl => cgl.UpdatedBy).HasColumnName("updated_by");
            builder.Property(cgl => cgl.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(cgl => cgl.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(cgl => cgl.CompanyId).HasColumnName("company_id").IsRequired();
            builder.HasOne(cgl => cgl.Company).WithOne(c => c.CompanyGeolocation)
                .HasForeignKey<CompanyGeolocation>(cgl => cgl.CompanyId);
        }
    }
}
