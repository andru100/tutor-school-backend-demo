using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Model;


public class ResendRequest
{
    public string email { get; set; }
}

public class SignUpData {
    public string Email {get; set;}
    public string Password {get; set;}
    public bool RememberMe {get; set;}
}



public class AdminCreateTeacherData
{
    public string? name { get; set; }
    public DateTime? dob { get; set; }
    public string? PhoneNumber { get; set; }
    public string? email { get; set; }
    public string? stream { get; set; }
    public string? sortCode { get; set; }
    public string? accountNo { get; set; }
    public string? notes { get; set; }
    public string? newPassword { get; set; }
    public string? oldPassword { get; set; }
     //public bool   rememberMe { get; set; }
}

public class AdminCreateStudentData
{
    public string? teacherId { get; set; }
    public string? name { get; set; }
    public DateTime? dob { get; set; }
    public string? parentName { get; set; }
    public string? PhoneNumber { get; set; } 
    public string? email { get; set; }
    public string? sortCode { get; set; }
    public string? accountNo { get; set; }
    public string? stream { get; set; }
    public string? adjustments { get; set; }
    public string? notes { get; set; }
    public string? newPassword { get; set; }
    public string? oldPassword { get; set; }
   //  public bool   rememberMe { get; set; }
}

public class ApplicationUser : IdentityUser
{

    public ApplicationUser()
    {
    }

    public DateTime? Dob { get; set; }
    public string? Name { get; set; } 
    public string? SortCode { get; set; }
    public string? AccountNo { get; set; }
    public string? Stream { get; set; }
    public string? Adjustments { get; set; }
    public string? Notes { get; set; }
    public string? ParentName { get; set; }

   
    public  Teacher? Teacher { get; set; }
    public  Student? Student { get; set; }


}


public class Teacher
{
    public string teacherId { get; set; }
    public string? name { get; set; }
    public string? messages { get; set; }
    public string? notes { get; set; }
    public string? profileImgUrl {get; set;}

    
    public ApplicationUser? applicationUser { get; set; }
    public ICollection<Student>? students { get; set; }
    public ICollection<LessonEvent>? lessonEvents { get; set; }
    public List<CalendarEvent>? calendarEvents { get; set; }
    public ICollection<HomeworkAssignment>? homeworkAssignments { get; set; }
    public ICollection<StudentAssessment>? assessments { get; set; } 

    
}
 
public class Student
{
    public string studentId { get; set; }
    public string? name { get; set; }
    public string? teacherId { get; set; }
    public string? stream { get; set; }
    public string? messages { get; set; }
    public string? stats { get; set; }
    public string? notes { get; set; }
    public string? profileImgUrl {get; set;}

    
    public ApplicationUser? applicationUser { get; set; }
    
    public Teacher? teacher { get; set; }
    public ICollection<LessonEvent>? lessonEvents { get; set; }
    public ICollection<CalendarEvent>? calendarEvents { get; set; }
    public ICollection<HomeworkAssignment>? homeworkAssignments { get; set; }
    public ICollection<StudentAssessment>? assessments { get; set; } 
    
}

public class Event
{
    public int? id { get; set; }
    public string title { get; set; }
    public string? description { get; set; }
    public DateTime dueDate   { get; set; }

    
    public CalendarEvent? calendarEvents { get; set; }
}


public class LessonEvent : Event
{
    public string? teacherId { get; set; }
    public string studentId { get; set; }
    public string? links { get; set; }
    public bool isAssigned { get; set; }
    public bool isComplete { get; set; }
    public DateTime? completionDate { get; set; }

    
    public Teacher? teacher { get; set; }
    public Student? student { get; set; }
}



public class StudentAssessment : Event
{
    public string? teacherId { get; set; }
    public string studentId { get; set; }
    public int assessmentId { get; set; }
    public bool isAssigned { get; set; }
    public bool isSubmitted { get; set; }
    public bool isGraded { get; set; }
    public DateTime? gradedDate { get; set; }
    public int? duration { get; set; }
    public double? score { get; set; }
    public DateTime? submissionDate { get; set; }
    public TopicScores? topicScores { get; set; }    
    public List<ExamAnswer>? answers { get; set; }

    
    public Student? student { get; set; }
    public Teacher? teacher { get; set; }

    public StudentAssessment()
    {
        answers = new List<ExamAnswer>();
        topicScores = new TopicScores();
    }
    
}

public enum HomeworkStream
{
    GCSE,
    A_Level,
    SATs,
    ElevenPlus,
    IB,
    BTEC,
    ScottishHighers,
    WelshBaccalaureate
}

public enum SubmissionContentType
{
    Text,
    Image,
    Document
}

public class HomeworkAssignment : Event
{
    public string? teacherId { get; set; }
    public string studentId { get; set; }
    public HomeworkStream stream { get; set; } //store the topic/level for appropriate ai feedback
    public bool isAssigned { get; set; }
    public bool isSubmitted { get; set; }
    public bool isGraded { get; set; }
    public DateTime? gradedDate { get; set; }
    public int? grade { get; set; }
    public string? aiFeedback { get; set; }
    public string? teacherFeedback { get; set; }
    public DateTime? submissionDate { get; set; }
    public string? submissionContent { get; set; } 
    public SubmissionContentType? submissionContentType { get; set; } 
    

    
    public Teacher? teacher { get; set; }
    public Student? student { get; set; }
}




public class CalendarEvent 
{
    public int? id { get; set; }
    public string? teacherId { get; set; }
    public string? studentId { get; set; }
    public string title { get; set; }
    public string? description { get; set; }
    public int eventId { get; set; }
    public DateTime date { get; set; }
    public string? link { get; set; }
    public string status { get; set; }

    
    public Teacher? teacher { get; set; }
    public Student? student { get; set; }
    public Event? Event { get; set; }

}

public class LoginResult
{
    public bool Successful { get; set; }
    public string Token { get; set; }
    public string Error { get; set; }
}


public class StudentGet 
{
    public string? id { get; set; }
    public string? auth { get; set; }
}

public class GetStudents 
{
    public string? teacherId { get; set; }
    public IEnumerable<string>? students { get; set; }
    public string? auth { get; set; }
}

public class GetAssessments
{
    public int? assessmentId { get; set; }
    public IEnumerable<string>? students { get; set; }
    public string? auth { get; set; }
}

public class GetAssignments
{
    public int? assignmentId { get; set; }
    public IEnumerable<string>? students { get; set; }
    public string? auth { get; set; }
}

public class AssignAssessment
{
    public int assessmentId { get; set; }
    public string studentId { get; set; }
    public string? teacherId { get; set; }
    public bool isAssigned { get; set; }
    public bool isSubmitted { get; set; }
    public bool isGraded { get; set; }
    public string? auth { get; set; }
}



public class TeacherGet
{
    public string? id { get; set; }
    public string? auth { get; set; }
}

public class GoogleTokenData
{
    public string Credential { get; set; }
    public string? ClientId { get; set; }
    public string? SelectBy { get; set; }
    public string? Role {get; set;}
}




public class AdminCreateStudent
{
    public AdminCreateStudentData authDetails { get; set; }
    public Student studentDetails { get; set; }
}

public class AdminCreateTeacher
{
    public AdminCreateTeacherData authDetails { get; set; }
    public Teacher teacherDetails { get; set; }
}

public class AdminDelete
{
    public string id { get; set; }
    public string? auth { get; set; }
}

public class HomeworkDelete
{
    public int id { get; set; }
    public string? auth { get; set; }
}

public class CalendarDelete
{
    public int id { get; set; }
    public string? auth { get; set; }
}

public class LessonDelete
{
    public int id { get; set; }
    public string? auth { get; set; }
}

public class AssignmentDelete
{
    public int id { get; set; }
    public string? auth { get; set; }
}

public class AssessmentDelete
{
    public int id { get; set; }
    public string? auth { get; set; }
}

public class GenerateInfo
{
    public int id { get; set; }
}

public class MutationResponse
{
    public bool success { get; set; }
}

public class HomeworkUploadTxtData
{
    public string text { get; set; }
    public int id { get; set; }
}

public class HomeworkFeedbackInput
{
    public string stream { get; set; }
    public string instructions { get; set; }
    public string submission { get; set; }
}

public class HomeworkFeedback
{
    public string positives { get; set; }
    public string negatives { get; set; }
    public string suggestions { get; set; }
}


public class Assessment
{
    public int id { get; set; }
    public string title { get; set; }
    public string stream { get; set; }
    public ICollection<Question> questionsWithAnswers { get; set; }
}

public class Question
{
    public int id { get; set; }
    public int? parentId { get; set; } // Id of the parent question
    public int assessmentId { get; set; }
    public string? media { get; set; }
    public string? code { get; set; }
    public string topic { get; set; }
    public QuestionType questionType { get; set; }
    public string questionText { get; set; }
    public List<AnswerOption> answerOptions { get; set; }
    public DateTime creationDate { get; set; }
    public VerificationStatus verificationStatus { get; set; }
    public List<VerificationRecord> verifiedHumanFeedback { get; set; }
    public List<Question> derivedQuestions { get; set; } // List of questions derived from this question
}

public enum QuestionType
{
    Original,
    Alt,
    Synthetic
}


public enum VerificationStatus
{
    Unverified,
    Verified,
    FlaggedForReview,
    Rejected
}


public class VerificationRecord
{
    public int? id { get; set; }
    public int questionBankId { get; set; }
    public bool isVerified { get; set; }
    public string? verifierId { get; set; }
    public DateTime? verificationDate { get; set; }
    public string notes { get; set; }
}

public class AnswerOption
{
    public int answerId { get; set; }
    public int questionId { get; set; }
    public string answerText { get; set; }
    public bool isCorrect { get; set; }

    public Question quizQuestion { get; set; } 
}


public class SeedAssessmentRequest
{
    public string teacherId { get; set; }
    public string studentId { get; set; } 
    public Assessment assessmentInfo { get; set; }
    public List<Question> questions { get; set; }
}



public class ExamAnswer
{
    public int questionId { get; set; }
    public int answerId { get; set; }
}

public class TopicScores
{
    public EnglishScores? english { get; set; }
    public MathScores? math { get; set; }

    public TopicScores()
    {
        english = new EnglishScores();
        math = new MathScores();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "english", english?.ToDictionary() },
            { "math", math?.ToDictionary() }
        };
    }
}

public class EnglishScores
{
    public Score? grammar_and_punctuation { get; set; }
    public Score? reading_and_comprehension { get; set; }
    public Score? spelling_and_vocabulary { get; set; }
    public Score? writing_skills { get; set; }
    public Score? comprehension_and_analysis_of_literature { get; set; }
    public Score? sentence_completion { get; set; }
    public Score? sentence_transformation { get; set; }
    public Score? literary_terms_and_concepts { get; set; }
    public Score? grammar_rules_and_usage { get; set; }

    public EnglishScores()
    {
        grammar_and_punctuation = new Score();
        reading_and_comprehension = new Score();
        spelling_and_vocabulary = new Score();
        writing_skills = new Score();
        comprehension_and_analysis_of_literature = new Score();
        sentence_completion = new Score();
        sentence_transformation = new Score();
        literary_terms_and_concepts = new Score();
        grammar_rules_and_usage = new Score();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "grammar_and_punctuation", grammar_and_punctuation?.ToDictionary() },
            { "reading_and_comprehension", reading_and_comprehension?.ToDictionary() },
            { "spelling_and_vocabulary", spelling_and_vocabulary?.ToDictionary() },
            { "writing_skills", writing_skills?.ToDictionary() },
            { "comprehension_and_analysis_of_literature", comprehension_and_analysis_of_literature?.ToDictionary() },
            { "sentence_completion", sentence_completion?.ToDictionary() },
            { "sentence_transformation", sentence_transformation?.ToDictionary() },
            { "literary_terms_and_concepts", literary_terms_and_concepts?.ToDictionary() },
            { "grammar_rules_and_usage", grammar_rules_and_usage?.ToDictionary() }
        };
    }
}

public class MathScores
{
    public Score? algebra { get; set; }
    public Score? addition { get; set; }
    public Score? subtraction { get; set; }
    public Score? multiplication { get; set; }
    public Score? division { get; set; }
    public Score? fractions { get; set; }
    public Score? percentages { get; set; }
    public Score? decimals { get; set; }
    public Score? ratio_and_proportion { get; set; }
    public Score? number_and_place_value {get; set;}
    public Score? measurement { get; set; }
    public Score? statistics { get; set; }

    public MathScores()
    {
        algebra = new Score();
        addition = new Score();
        subtraction = new Score();
        multiplication = new Score();
        division = new Score();
        fractions = new Score();
        percentages = new Score();
        decimals = new Score();
        ratio_and_proportion = new Score();
        number_and_place_value = new Score();
        measurement = new Score();
        statistics = new Score();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "algebra", algebra?.ToDictionary() },
            { "addition", addition?.ToDictionary() },
            { "subtraction", subtraction?.ToDictionary() },
            { "multiplication", multiplication?.ToDictionary() },
            { "division", division?.ToDictionary() },
            { "fractions", fractions?.ToDictionary() },
            { "percentages", percentages?.ToDictionary() },
            { "decimals", decimals?.ToDictionary() },
            { "ratio_and_proportion", ratio_and_proportion?.ToDictionary() },
            { "number_and_place_value", number_and_place_value.ToDictionary()},
            { "measurement", measurement?.ToDictionary() },
            { "statistics", statistics?.ToDictionary() }
        };
    }
}


public class Score
{
    public double? total { get; set; }
    public double? correct { get; set; }
    public double? score { get; set; }

    public Score()
    {
        correct = 0;
        total = 0;
        score = 0;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "total", total },
            { "correct", correct },
            { "score", score }
        };
    }
}


public class StripeOptions
{
    public string option { get; set; }
}



