using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Model.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teachers");

        builder.HasKey(t => t.teacherId);

        builder.Property(t => t.teacherId).HasColumnName("teacher_id");
        builder.Property(t => t.name).HasColumnName("name");
        builder.Property(t => t.messages).HasColumnName("messages");
        builder.Property(t => t.notes).HasColumnName("notes");
        builder.Property(t => t.profileImgUrl).HasColumnName("profileImgUrl");

        builder.HasOne(t => t.applicationUser)
               .WithOne(a => a.Teacher)
               .HasForeignKey<Teacher>(t => t.teacherId);
    }
}
