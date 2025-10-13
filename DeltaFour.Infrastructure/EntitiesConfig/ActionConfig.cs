using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Action = DeltaFour.Domain.Entities.Action;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class ActionConfig : IEntityTypeConfiguration<Action>
    {
        public void Configure(EntityTypeBuilder<Action> builder)
        {
            builder.ToTable("action");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Name).HasColumnName("name").IsUnicode(false).HasMaxLength(255).IsRequired();
            builder.HasMany(a => a.RolePermissions).WithOne(rp => rp.Action)
                .HasForeignKey(rp => rp.ActionId);
            builder.HasData(
                new Action { Name = "list" }, 
                new Action { Name = "create" }, 
                new Action { Name = "update" },
                new Action { Name = "delete" }
                );
        }
    }
}
