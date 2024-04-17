using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Model;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options) { }


    public DbSet<Teacher> teachers { get; set; }
    public DbSet<Student> students { get; set; }
    public DbSet<Event> events { get; set; }
    public DbSet<HomeworkAssignment> homework_assignments { get; set; }
    public DbSet<LessonEvent> lesson_events { get; set; }
    public DbSet<CalendarEvent> calendar_events { get; set; }
    public DbSet<Assessment> assessments { get; set; }
    public DbSet<Question> exam_questions { get; set; }
    public DbSet<VerificationRecord> VerificationRecords { get; set; }
    public DbSet<StudentAssessmentAssignment> student_assessment_assignment { get; set; } 

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Name).HasColumnName("Name");
            entity.Property(u => u.Dob).HasColumnName("Dob");
            // entity.Property(u => u.SortCode).HasColumnName("SortCode");
            // entity.Property(u => u.AccountNo).HasColumnName("AccountNo");
            entity.Property(u => u.Stream).HasColumnName("Stream");
            entity.Property(u => u.Adjustments).HasColumnName("Adjustments");
            entity.Property(u => u.Notes).HasColumnName("Notes");
            entity.Property(u => u.ParentName).HasColumnName("ParentName");
        });
            
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("teachers");
            entity.Property(e => e.teacherId).HasColumnName("teacher_id");
            entity.Property(e => e.name).HasColumnName("name");
            entity.Property(e => e.messages).HasColumnName("messages");
            entity.Property(e => e.notes).HasColumnName("notes");
            entity.Property(e => e.profileImgUrl).HasColumnName("profileImgUrl");

            entity.HasKey(e => e.teacherId);
            
            entity.HasOne(t => t.applicationUser)
                .WithOne(a => a.Teacher)
                .HasForeignKey<Teacher>(t => t.teacherId);

        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");
            entity.Property(e => e.studentId).HasColumnName("student_id");
            entity.Property(e => e.name).HasColumnName("name");
            entity.Property(e => e.teacherId).HasColumnName("teacher_id");
            entity.Property(e => e.stream).HasColumnName("stream");
            entity.Property(e => e.messages).HasColumnName("messages");
            entity.Property(e => e.stats).HasColumnName("stats");
            entity.Property(e => e.notes).HasColumnName("notes");
            entity.Property(e => e.profileImgUrl).HasColumnName("profileImgUrl");
            entity.HasKey(e => e.studentId);

            
            entity.HasOne(e => e.applicationUser)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(e => e.studentId);
                
            entity.HasOne(s => s.teacher)
                .WithMany(t => t.students)
                .HasForeignKey(s => s.teacherId);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.id); 
        });

        modelBuilder.Entity<HomeworkAssignment>(entity =>
        {
            entity.ToTable("homework");
            entity.Property(e => e.teacherId).HasColumnName("teacher_id");
            entity.Property(e => e.studentId).HasColumnName("student_id");
            entity.Property(e => e.stream).HasColumnName("stream");
            entity.Property(e => e.isAssigned).HasColumnName("is_assigned");
            entity.Property(e => e.isSubmitted).HasColumnName("is_submitted");
            entity.Property(e => e.isGraded).HasColumnName("is_graded");
            entity.Property(e => e.grade).HasColumnName("grade");
            entity.Property(e => e.teacherFeedback).HasColumnName("teacher_feedback");
            entity.Property(e => e.aiFeedback).HasColumnName("ai_feedback");
            entity.Property(e => e.submissionDate).HasColumnName("submission_date");
            entity.Property(e => e.submissionContent).HasColumnName("submission_content");

            entity.HasOne(a => a.teacher).WithMany(s => s.homeworkAssignments).HasForeignKey(a => a.teacherId);
            entity.HasOne(a => a.student).WithMany(s => s.homeworkAssignments).HasForeignKey(a => a.studentId);
        });

        modelBuilder.Entity<LessonEvent>(entity =>
        {
            entity.ToTable("lesson_events");
            entity.Property(e => e.teacherId).HasColumnName("teacher_id");
            entity.Property(e => e.studentId).HasColumnName("student_id");
            entity.Property(e => e.links).HasColumnName("links");
            entity.Property(e => e.isAssigned).HasColumnName("is_assigned");
            entity.Property(e => e.isComplete).HasColumnName("is_complete");
            entity.Property(e => e.completionDate).HasColumnName("completion_date");

            entity.HasOne(e => e.teacher).WithMany(s => s.lessonEvents).HasForeignKey(e => e.teacherId);
            entity.HasOne(e => e.student).WithMany(s => s.lessonEvents).HasForeignKey(e => e.studentId);
        });

        modelBuilder.Entity<StudentAssessmentAssignment>(entity =>
        {
            entity.ToTable("student_assessment_assignment");
            entity.Property(e => e.studentId).HasColumnName("student_id");
            entity.Property(e => e.teacherId).HasColumnName("teacher_id");
            entity.Property(e => e.assessmentId).HasColumnName("assessment_id");
            entity.Property(e => e.isAssigned).HasColumnName("is_assigned");
            entity.Property(e => e.isSubmitted).HasColumnName("is_submitted");
            entity.Property(e => e.submissionDate).HasColumnName("submission_date");
            entity.Property(e => e.isGraded).HasColumnName("is_graded");
            entity.Property(e => e.score).HasColumnName("score");
            entity.Property(e => e.duration).HasColumnName("duration");

            entity.HasOne(e => e.teacher).WithMany(s => s.assessments).HasForeignKey(e => e.teacherId);
            entity.HasOne(e => e.student).WithMany(s => s.assessments).HasForeignKey(e => e.studentId);

            entity.Property(e => e.answers)
                .HasColumnName("answers")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<ExamAnswer>>(v));

            entity.Property(e => e.topicScores)
                .HasColumnName("topic_scores")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<TopicScores>(v));
 
        });

        modelBuilder.Entity<CalendarEvent>(entity =>
        {
            entity.ToTable("calendar");
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.teacherId).HasColumnName("teacher_id");
            entity.Property(e => e.studentId).HasColumnName("student_id");
            entity.Property(e => e.title).HasColumnName("title");
            entity.Property(e => e.description).HasColumnName("description");
            entity.Property(e => e.eventId).HasColumnName("event_id");
            entity.Property(e => e.date).HasColumnName("date");
            entity.Property(e => e.link).HasColumnName("link");
            entity.Property(e => e.status).HasColumnName("status");
            
            entity.HasKey(e => e.id);

            entity.HasOne(e => e.teacher).WithMany(t => t.calendarEvents).HasForeignKey(e => e.teacherId);
            entity.HasOne(e => e.student).WithMany(s => s.calendarEvents).HasForeignKey(e => e.studentId); 
            entity.HasOne(e => e.Event).WithOne(s => s.calendarEvents).HasForeignKey<CalendarEvent>(e => e.eventId);
            

        });


        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.ToTable("assessment");
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.title).HasColumnName("title");
            entity.Property(e => e.stream).HasColumnName("stream");
            entity.HasKey(e => e.id);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("exam_questions");
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.parentId).HasColumnName("parent_id");
            entity.Property(e => e.assessmentId).HasColumnName("assessment_id");
            entity.Property(e => e.media).HasColumnName("media");
            entity.Property(e => e.code).HasColumnName("code");
            entity.Property(e => e.topic).HasColumnName("topic");
            entity.Property(e => e.questionType).HasColumnName("question_type");
            entity.Property(e => e.questionText).HasColumnName("question_text");
            entity.Property(e => e.creationDate).HasColumnName("creation_date");
            entity.Property(e => e.verificationStatus).HasColumnName("verification_status");

            entity.HasMany(q => q.derivedQuestions)
                .WithOne()
                .HasForeignKey(q => q.parentId);

            entity.HasMany(q => q.verifiedHumanFeedback)
                .WithOne()
                .HasForeignKey(vr => vr.questionBankId);

            entity.Property(e => e.answerOptions)
                .HasColumnName("answer_options")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<AnswerOption>>(v));
        });


        modelBuilder.Entity<VerificationRecord>(entity =>
        {
            entity.ToTable("verification_records");
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.questionBankId).HasColumnName("question_bank_id");
            entity.Property(e => e.isVerified).HasColumnName("is_verified");
            entity.Property(e => e.verifierId).HasColumnName("verifier_id");
            entity.Property(e => e.verificationDate).HasColumnName("verification_date");
            entity.Property(e => e.notes).HasColumnName("notes");
            entity.HasKey(e => e.id); 

            entity.HasOne<Question>()
                .WithMany()
                .HasForeignKey(e => e.id)
                .IsRequired();

            entity.HasOne<Teacher>()
                .WithMany()
                .HasForeignKey(e => e.verifierId)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}