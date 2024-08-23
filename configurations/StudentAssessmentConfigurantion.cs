using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Model;

namespace Model.Configurations;

public class StudentAssessmentConfiguration : IEntityTypeConfiguration<StudentAssessment>
{
    public void Configure(EntityTypeBuilder<StudentAssessment> builder)
    {
        builder.ToTable("student_assessment");

        builder.Property(e => e.studentId).HasColumnName("student_id");
        builder.Property(e => e.teacherId).HasColumnName("teacher_id");
        builder.Property(e => e.assessmentId).HasColumnName("assessment_id");
        builder.Property(e => e.isAssigned).HasColumnName("is_assigned");
        builder.Property(e => e.isSubmitted).HasColumnName("is_submitted");
        builder.Property(e => e.submissionDate).HasColumnName("submission_date");
        builder.Property(e => e.isGraded).HasColumnName("is_graded");
        builder.Property(e => e.gradedDate).HasColumnName("gradedDate");
        builder.Property(e => e.score).HasColumnName("score");
        builder.Property(e => e.duration).HasColumnName("duration");

        builder.HasOne(e => e.teacher)
               .WithMany(s => s.assessments)
               .HasForeignKey(e => e.teacherId);

        builder.HasOne(e => e.student)
               .WithMany(s => s.assessments)
               .HasForeignKey(e => e.studentId);

        builder.Property(e => e.answers)
               .HasColumnName("answers")
               .HasJsonConversion<List<ExamAnswer>>();

        builder.Property(e => e.topicScores)
               .HasColumnName("topic_scores")
               .HasJsonConversion<TopicScores>();
    }
}