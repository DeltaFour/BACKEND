using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class AddressConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("address");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.City).IsRequired().IsUnicode().HasMaxLength(255).HasColumnName("city");
            builder.Property(a => a.Street).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("street");
            builder.Property(a => a.District).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("district");
            builder.Property(a => a.State).IsRequired().HasMaxLength(2).HasColumnName("state");
            builder.Property(a => a.ZipCode).IsRequired().HasMaxLength(9).HasColumnName("zip_code");
            builder.Property(a => a.Number).IsRequired().HasColumnName("number");
            builder.Property(a => a.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.HasOne(a => a.Company).WithOne(c => c.Address).HasForeignKey<Company>(c => c.AddressId);
        }
    }
}
