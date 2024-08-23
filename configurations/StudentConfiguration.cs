using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Model.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");

        builder.HasKey(s => s.studentId);

        builder.Property(s => s.studentId).HasColumnName("student_id");
        builder.Property(s => s.name).HasColumnName("name");
        builder.Property(s => s.teacherId).HasColumnName("teacher_id");
        builder.Property(s => s.stream).HasColumnName("stream");
        builder.Property(s => s.messages).HasColumnName("messages");
        builder.Property(s => s.stats).HasColumnName("stats");
        builder.Property(s => s.notes).HasColumnName("notes");
        builder.Property(s => s.profileImgUrl).HasColumnName("profileImgUrl");

        builder.HasOne(s => s.applicationUser)
               .WithOne(a => a.Student)
               .HasForeignKey<Student>(s => s.studentId);

        builder.HasOne(s => s.teacher)
               .WithMany(t => t.students)
               .HasForeignKey(s => s.teacherId);
    }
}
