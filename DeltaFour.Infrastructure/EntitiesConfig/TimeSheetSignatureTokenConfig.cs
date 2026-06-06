using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class TimeSheetSignatureTokenConfig : IEntityTypeConfiguration<TimeSheetSignatureToken>
{
    public void Configure(EntityTypeBuilder<TimeSheetSignatureToken> builder)
    {
        builder.ToTable("time_sheet_signature_tokens");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.TimeSheetId)
            .IsRequired()
            .HasColumnName("time_sheet_id");

        builder.Property(e => e.SignerType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnName("signer_type");

        builder.Property(e => e.SignerUserId)
            .IsRequired()
            .HasColumnName("signer_user_id");

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnName("token");

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email");

        builder.Property(e => e.ExpiresAtUtc)
            .IsRequired()
            .HasColumnName("expires_at_utc");

        builder.Property(e => e.UsedAtUtc)
            .HasColumnName("used_at_utc");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("IX_time_sheet_signature_tokens_token");

        builder.HasOne(e => e.TimeSheet)
            .WithMany(t => t.SignatureTokens)
            .HasForeignKey(e => e.TimeSheetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
