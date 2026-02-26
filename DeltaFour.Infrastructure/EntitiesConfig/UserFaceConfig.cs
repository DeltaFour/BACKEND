using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class UserFaceConfig : IEntityTypeConfiguration<UserFace>
    {
        public void Configure(EntityTypeBuilder<UserFace> builder)
        {
            builder.ToTable("user_face");
            builder.HasKey(ef => ef.Id);
            builder.Property(ef => ef.Id).HasColumnName("id");
            builder.Property(ef => ef.UserId).HasColumnName("employee_id").IsRequired();
            builder.Property(ef => ef.FaceTemplate).IsRequired();
            builder.Property(ef => ef.UpdatedAt).HasColumnName("updated_at");
            builder.Property(ef => ef.UpdatedBy).HasColumnName("updated_by");
            builder.Property(ef => ef.CreatedBy).HasColumnName("created_by").IsRequired();
            builder.Property(ef => ef.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.HasOne<User>(ef => ef.User).WithMany(e => e.UserFaces)
                .HasForeignKey(e => e.UserId);
        }
    }
}
