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
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.Name).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("name");
            builder.Property(c => c.Cnpj).IsRequired().HasMaxLength(18).HasColumnName("cnpj");
            builder.Property(c => c.IsActive).IsRequired().HasColumnName("is_active");
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(c => c.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
            builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");
            builder.Property(c => c.AddressId).HasColumnName("address_id");
            builder.HasMany(c => c.Employees).WithOne(u => u.Company).HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Address>(c => c.Address).WithOne(a => a.Company).HasForeignKey<Company>(c => c.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Roles).WithOne(r => r.Company).HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.CompanyGeolocation).WithOne(cgl => cgl.Company)
                .HasForeignKey<CompanyGeolocation>(cgl => cgl.CompanyId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
