using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Infrastructure.EntitiesConfig
{
    public class UserAuthConfig: IEntityTypeConfiguration<UserAuth>
    {
        public void Configure(EntityTypeBuilder<UserAuth> builder)
        {
            builder.ToTable("user_auth");
            builder.HasKey(ua => ua.Id);
            builder.Property(ua => ua.RefreshToken)
                .IsRequired();
            builder.Property(ua => ua.ExpiresAt)
                .IsRequired();
            builder.HasOne(au => au.User)
                .WithOne(u => u.UserAuth)
                .HasForeignKey<UserAuth>(ua => ua.UserId);
        }

    }
}
