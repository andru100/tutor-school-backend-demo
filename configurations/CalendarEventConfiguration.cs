using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Model;

namespace Model.Configurations;

public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("calendar");

        builder.HasKey(e => e.id);

        builder.Property(e => e.id).HasColumnName("id");
        builder.Property(e => e.teacherId).HasColumnName("teacher_id");
        builder.Property(e => e.studentId).HasColumnName("student_id");
        builder.Property(e => e.title).HasColumnName("title");
        builder.Property(e => e.description).HasColumnName("description");
        builder.Property(e => e.eventId).HasColumnName("event_id");
        builder.Property(e => e.date).HasColumnName("date");
        builder.Property(e => e.link).HasColumnName("link");
        builder.Property(e => e.status).HasColumnName("status");

        builder.HasOne(e => e.teacher)
               .WithMany(t => t.calendarEvents)
               .HasForeignKey(e => e.teacherId);
        builder.HasOne(e => e.student)
               .WithMany(s => s.calendarEvents)
               .HasForeignKey(e => e.studentId);
        builder.HasOne(e => e.Event)
               .WithOne(s => s.calendarEvents)
               .HasForeignKey<CalendarEvent>(e => e.eventId);
    }
}