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
            builder.Property(l => l.Name).HasColumnName("name").IsRequired();
        }
    }
}
