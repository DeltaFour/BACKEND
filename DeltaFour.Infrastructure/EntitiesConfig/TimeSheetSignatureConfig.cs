using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeltaFour.Infrastructure.EntitiesConfig;

public class TimeSheetSignatureConfig : IEntityTypeConfiguration<TimeSheetSignature>
{
    public void Configure(EntityTypeBuilder<TimeSheetSignature> builder)
    {
        builder.ToTable("time_sheet_signatures");

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

        builder.Property(e => e.SignerName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("signer_name");

        builder.Property(e => e.SignerCpf)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("signer_cpf");

        builder.Property(e => e.SignerEmail)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("signer_email");

        builder.Property(e => e.SignedAtUtc)
            .IsRequired()
            .HasColumnName("signed_at_utc");

        builder.Property(e => e.SignerIp)
            .IsRequired()
            .HasMaxLength(45)
            .HasColumnName("signer_ip");

        builder.Property(e => e.TimeSheetHash)
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnName("time_sheet_hash");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .HasColumnName("created_at");

        builder.HasOne(e => e.TimeSheet)
            .WithMany(t => t.Signatures)
            .HasForeignKey(e => e.TimeSheetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
