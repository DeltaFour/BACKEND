using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(256);
        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        builder
            .Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(256);
        builder
            .Property(u => u.Role)
            .IsRequired();
    }
}