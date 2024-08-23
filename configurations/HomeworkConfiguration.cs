

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Model.Configurations;

public class HomeworkAssignmentConfiguration : IEntityTypeConfiguration<HomeworkAssignment>
{
    public void Configure(EntityTypeBuilder<HomeworkAssignment> builder)
    {
        builder.ToTable("homework");

        builder.Property(h => h.teacherId).HasColumnName("teacher_id");
        builder.Property(h => h.studentId).HasColumnName("student_id");
        builder.Property(h => h.stream).HasColumnName("stream");
        builder.Property(h => h.isAssigned).HasColumnName("is_assigned");
        builder.Property(h => h.isSubmitted).HasColumnName("is_submitted");
        builder.Property(h => h.isGraded).HasColumnName("is_graded");
        builder.Property(h => h.gradedDate).HasColumnName("gradedDate");
        builder.Property(h => h.grade).HasColumnName("grade");
        builder.Property(h => h.teacherFeedback).HasColumnName("teacher_feedback");
        builder.Property(h => h.aiFeedback).HasColumnName("ai_feedback");
        builder.Property(h => h.submissionDate).HasColumnName("submission_date");
        builder.Property(h => h.submissionContent).HasColumnName("submission_content");

        builder.HasOne(h => h.teacher)
               .WithMany(t => t.homeworkAssignments)
               .HasForeignKey(h => h.teacherId);
        builder.HasOne(h => h.student)
               .WithMany(s => s.homeworkAssignments)
               .HasForeignKey(h => h.studentId);
    }
}