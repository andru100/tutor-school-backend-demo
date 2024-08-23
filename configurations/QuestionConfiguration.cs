using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Model;

namespace Model.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("exam_questions");

        builder.HasKey(q => q.id);

        builder.Property(q => q.id).HasColumnName("id");
        builder.Property(q => q.parentId).HasColumnName("parent_id");
        builder.Property(q => q.assessmentId).HasColumnName("assessment_id");
        builder.Property(q => q.media).HasColumnName("media");
        builder.Property(q => q.code).HasColumnName("code");
        builder.Property(q => q.topic).HasColumnName("topic");
        builder.Property(q => q.questionType).HasColumnName("question_type");
        builder.Property(q => q.questionText).HasColumnName("question_text");
        builder.Property(q => q.creationDate).HasColumnName("creation_date");
        builder.Property(q => q.verificationStatus).HasColumnName("verification_status");

        builder.HasMany(q => q.derivedQuestions)
            .WithOne()
            .HasForeignKey(q => q.parentId);

        builder.HasMany(q => q.verifiedHumanFeedback)
            .WithOne()
            .HasForeignKey(vr => vr.questionBankId);

        builder.Property(q => q.answerOptions)
            .HasColumnName("answer_options")
            .HasJsonConversion();
    }
}