using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(u => u.Id);
        builder
            .Property(u => u.Name)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(255);
        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
        builder
            .Property(u => u.Password)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(255);
        builder
            .Property(u => u.Cellphone)
            .HasMaxLength(14);
        builder
            .Property(u=> u.IsActive)
            .IsRequired();
        builder
            .Property(u => u.IsConfirmed)
            .IsRequired();
        builder
            .Property(u => u.IsAllowedBypassCoord)
            .IsRequired();
        builder
            .Property(u => u.LastLogin);
        builder
            .Property(u => u.UpdatedAt);
        builder
            .Property(u => u.UpdatedBy);
        builder
            .Property(u => u.CreatedBy)
            .IsRequired();
        builder.Property(u => u.CreatedAt)
            .IsRequired();
        builder
            .HasOne<Company>(u => u.Company)
            .WithMany(c => c.Users)
            .IsRequired()
            .HasForeignKey(u => u.CompanyId);
        builder.HasOne<UserAuth>(u => u.UserAuth)
            .WithOne(ua => ua.User)
            .HasForeignKey<UserAuth>(ua => ua.UserId);
        builder.HasOne<Role>(u => u.Role)
            .WithOne(r => r.User)
            .HasForeignKey<User>(r => r.RoleId);
    }
}