using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class AddressConfing : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("address");
            builder.HasKey(a => a.Id);
            builder
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(255);
            builder
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(255);
            builder
                .Property(a => a.District)
                .IsRequired()
                .HasMaxLength(255);
            builder
                .Property(a => a.State)
                .IsRequired()
                .HasMaxLength(2);
            builder
                .Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(9);
            builder
                .Property(a => a.Number)
                .IsRequired(); 
            builder
                .Property(a => a.CreatedAt)
                .IsRequired();
            builder
                .HasOne(a => a.Company)
                .WithOne(c => c.Address)
                .HasForeignKey<Company>(c => c.AddressId);
        }
    }
}
