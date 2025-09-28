using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("company");
            builder.HasKey(c => c.Id);
            builder
                .Property(c => c.Name)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(255);
            builder
                .Property(c => c.Cnpj)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(18);
            builder
                .Property(c => c.IsActive)
                .IsRequired();
            builder
                .Property(c => c.CreatedAt)
                .IsRequired();
            builder
                .Property(c => c.UpdatedAt);
            builder
                .HasMany<User>(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId);
            builder
                .HasOne<Address>(c => c.Address)
                .WithOne(a => a.Company)
                .HasForeignKey<Company>(c => c.AddressId);
            builder
                .HasMany<Role>(c => c.Roles)
                .WithOne(r => r.Company)
                .HasForeignKey(r => r.CompanyId);
        }
    }
}
