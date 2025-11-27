using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("name");
        builder.Property(e => e.Email).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("email");
        builder.Property(e => e.Password).IsRequired().IsUnicode(false).HasMaxLength(255).HasColumnName("password");
        builder.Property(e => e.Cellphone).HasMaxLength(14).HasColumnName("cellphone");
        builder.Property(e => e.IsActive).IsRequired().HasColumnName("is_active");  
        builder.Property(e => e.IsConfirmed).IsRequired().HasColumnName("is_confirmed");
        builder.Property(e => e.IsAllowedBypassCoord).IsRequired().HasColumnName("is_allowed_by_pass_coord");
        builder.Property(e => e.LastLogin).HasColumnName("last_login");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder
            .Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");
        builder.Property(e => e.CompanyId).HasColumnName("company_id").IsRequired();
        builder.Property(e => e.RoleId).HasColumnName("role_id");
        builder.HasOne<Company>(e => e.Company).WithMany(c => c.User)
            .HasForeignKey(e => e.CompanyId);
        builder.HasOne<UserAuth>(e => e.UserAuth).WithOne(ea => ea.User)
            .HasForeignKey<UserAuth>(ea => ea.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Role).WithMany(r => r.User).HasForeignKey(e => e.RoleId);
        builder.HasMany(e => e.UserShifts).WithOne(es => es.User).HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.UserAttendances).WithOne(ea => ea.User)
            .HasForeignKey(ea => ea.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.UserFaces).WithOne(e => e.User)
            .HasForeignKey(ea => ea.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
