using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Model.Configurations;

public class LessonEventConfiguration : IEntityTypeConfiguration<LessonEvent>
{
    public void Configure(EntityTypeBuilder<LessonEvent> builder)
    {
        builder.ToTable("lesson_events");

        builder.Property(le => le.teacherId).HasColumnName("teacher_id");
        builder.Property(le => le.studentId).HasColumnName("student_id");
        builder.Property(le => le.links).HasColumnName("links");
        builder.Property(le => le.isAssigned).HasColumnName("is_assigned");
        builder.Property(le => le.isComplete).HasColumnName("is_complete");
        builder.Property(le => le.completionDate).HasColumnName("completion_date");

        builder.HasOne(le => le.teacher)
               .WithMany(t => t.lessonEvents)
               .HasForeignKey(le => le.teacherId);

        builder.HasOne(le => le.student)
               .WithMany(s => s.lessonEvents)
               .HasForeignKey(le => le.studentId);
    }
}