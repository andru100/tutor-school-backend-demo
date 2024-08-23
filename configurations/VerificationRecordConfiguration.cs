using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Model.Configurations;

public class VerificationRecordConfiguration : IEntityTypeConfiguration<VerificationRecord>
{
    public void Configure(EntityTypeBuilder<VerificationRecord> builder)
    {
        builder.ToTable("verification_records");

        builder.HasKey(v => v.id);

        builder.Property(v => v.id).HasColumnName("id");
        builder.Property(v => v.questionBankId).HasColumnName("question_bank_id");
        builder.Property(v => v.isVerified).HasColumnName("is_verified");
        builder.Property(v => v.verifierId).HasColumnName("verifier_id");
        builder.Property(v => v.verificationDate).HasColumnName("verification_date");
        builder.Property(v => v.notes).HasColumnName("notes");

        builder.HasOne<Question>()
            .WithMany()
            .HasForeignKey(v => v.id)
            .IsRequired();

        builder.HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(v => v.verifierId)
            .IsRequired();
    }
}