using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using Model.Configurations;

namespace Model;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
    {
        Database.SetCommandTimeout(120); 
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking; // Improve performance for read-only scenarios
    }

    public DbSet<Teacher> teachers { get; set; }
    public DbSet<Student> students { get; set; }
    public DbSet<Event> events { get; set; }
    public DbSet<HomeworkAssignment> homework_assignments { get; set; }
    public DbSet<LessonEvent> lesson_events { get; set; }
    public DbSet<CalendarEvent> calendar_events { get; set; }
    public DbSet<Assessment> assessments { get; set; }
    public DbSet<Question> exam_questions { get; set; }
    public DbSet<VerificationRecord> VerificationRecords { get; set; }
    public DbSet<StudentAssessment> student_assessment { get; set; } 

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseQuerySplitting();
    //     base.OnConfiguring(optionsBuilder);
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}