using Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AccountController;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace seed
{
    public class Seed
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        public Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserStore<ApplicationUser> userStore, ApplicationDbContext dbContext,  IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager; 
            _userStore = userStore;
            _dbContext = dbContext;
            _configuration = configuration;
        }
        
        public async Task SeedData([FromServices] IServiceProvider sp)
        {
            try{

                if (!await _roleManager.RoleExistsAsync("Teacher"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Teacher"));
                }
                if (!await _roleManager.RoleExistsAsync("Student"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Student"));
                }
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Create and sign in teacher
                var teacherId = await CreateAndSignInUser("teacher@example.com", "Password123!", "Teacher", sp);

                await CreateTeacherRecord(teacherId);

                // Create and sign in student
                var studentId = await CreateAndSignInUser("student@example.com", "Password123!", "Student", sp);
                await CreateStudentRecord(studentId, teacherId);

                // Create and seed math assessments
                var mathAssessment = new Assessment { title = "math", stream = "math" };
                var questionsClass = new generateQuestions();
                var mathQuestions = questionsClass.GenerateMathQuestions(); 
                SeedAssessment(teacherId, studentId, mathAssessment, mathQuestions);
                
                // Create and seed english assessments
                var englishAssessment = new Assessment { title = "english", stream = "english" };
                var englishQuestions = questionsClass.GenerateEnglishQuestions(); 
                SeedAssessment(teacherId, studentId, englishAssessment, englishQuestions);
                
                GenerateSeedData(teacherId, studentId);
            } catch(Exception ex) {
                Console.WriteLine($"SeedData failed, caught exception: {ex.Message}");
                throw new Exception($"SeedData caught an exception: {ex.Message}");
            }
        }

        private async Task<string> CreateAndSignInUser(string email, string password, string role, [FromServices] IServiceProvider sp)
        {
            try{
                if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
                {
                    throw new Exception("Invalid email address");
                }

                var user = new ApplicationUser();
                
                await _userStore.SetUserNameAsync(user, email, CancellationToken.None);

                //var userStore = sp.GetRequiredService<IUserStore<ApplicationUser>>();
                var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
                await emailStore.SetEmailAsync(user, email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);
                    // add teacher or student role and data record 
                    if (role == "Teacher")
                    {
                        await _userManager.AddToRoleAsync(user, "Teacher");
                        await SignUpTeacher(user.Id);
                    }
                    else if (role == "Student")
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                        await SignUpStudent(user.Id);
                    }
                    return user.Id;
                }

                throw new Exception($"Unable to create user, userManager.CreateAsync failed: {result.Errors.FirstOrDefault()?.Description}");
            }
            catch( Exception ex) {
                Console.WriteLine($"CreateAndSignInUser failed, caught exception: {ex.Message}");
                throw new Exception($"CreateAndSignInUser caught an exception: {ex.Message}");
            }
        }

        private async Task SignUpTeacher(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Name = "John Doe";
                user.Dob = DateTime.UtcNow;
                user.PhoneNumber = "0897171711";
                user.SortCode = "000400";
                user.AccountNo = "87890786";
                user.Notes = "These are notes";

                await _userManager.UpdateAsync(user);
            }
        }

        private async Task SignUpStudent(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Name = "Sarah Doe";
                user.Dob = DateTime.UtcNow;
                user.PhoneNumber = "08988871711";
                user.SortCode = "440400";
                user.AccountNo = "84490786";
                user.Notes = "These are notes";
                user.Stream = "11+";
                user.ParentName = "Mary conner";

                await _userManager.UpdateAsync(user);
            }
        }

        private async Task CreateTeacherRecord(string teacherId)
        {
            var teacher = new Teacher { teacherId = teacherId, name = "John Doe", notes = "These are notes" };
            _dbContext.teachers.Add(teacher);
            await _dbContext.SaveChangesAsync();
        }

        private async Task CreateStudentRecord(string studentId, string teacherId)
        {
            var student = new Student { studentId = studentId, teacherId = teacherId, name = "Sarah Doe", notes = "These are notes" };
            _dbContext.students.Add(student);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"created student record student id is: \n\n {studentId}");
        }

        public void SeedAssessment(string teacherId, string studentId, Assessment assessmentInfo, List<Question> questions)
        {
            try{
                if (teacherId == null)
                {
                    throw new Exception($"teacherId is null");
                }

                Console.WriteLine($"Seeding {assessmentInfo.title} for teacherId: {teacherId}");

                // Create a new assessment
                var assessment = new Assessment
                {
                    title = assessmentInfo.title,
                    stream = assessmentInfo.stream
                };

                // Add the assessment to the database to obtain the assessmentId
                _dbContext.assessments.Add(assessment);
                _dbContext.SaveChanges();

                int assessmentId = assessment.id;

                // Assign the assessmentId to each question and add them to the database
                foreach (var question in questions)
                {
                    question.assessmentId = assessmentId;
                    _dbContext.exam_questions.Add(question);
                }
                _dbContext.SaveChanges(); 

                // Assign the assessment to a student
                var student = _dbContext.students.FirstOrDefault(); 

                if (student != null)
                {
                    var studentAssessmentAssignment = new StudentAssessment
                    {
                        title = assessment.title.ToLower(),
                        studentId = studentId,
                        teacherId = teacherId,
                        assessmentId = assessmentId,
                        isAssigned = true,
                        isSubmitted = false,
                        isGraded = false,
                        dueDate = new DateTime(2023, new Random().Next(1, 13), new Random().Next(1, 29), 0, 0, 0, DateTimeKind.Utc),
                        score = null
                    };
                    
                    Console.WriteLine($"created studentassessmentassignment student id is: \n\n {studentId}");
                    _dbContext.student_assessment.Add(studentAssessmentAssignment);
                    _dbContext.SaveChanges(); 
                }
            } catch(Exception ex) {
                throw new Exception($"SeedAssessment failed: {ex.Message}");
            }
        }


        public void GenerateSeedData(string teacherId, string studentId)
        {
            try{
                var seedData = new List<StudentAssessment>();

                // Define the number of seed assessments to create in appsettings.json
                int NumberOfAssessments = Convert.ToInt32(_configuration["NUMBER_OF_ASSESSMENTS"]);

                Random random = new Random();

                DateTime pastDate = DateTime.UtcNow.AddDays(-30);

                for (int i = 1; i <= NumberOfAssessments; i++)
                {
                    pastDate = pastDate.AddMinutes(random.Next(60, 1440)); // Add random minutes to the past date

                    var assessment = new StudentAssessment
                    {
                        studentId = studentId,
                        teacherId = teacherId,
                        assessmentId = i % 2 == 0 ? 2 : 1, // Alternate between math (1) and English (2) assessments
                        isAssigned = true,
                        isSubmitted = true,
                        isGraded = true,
                        dueDate = pastDate.AddDays(random.Next(1, 30)), // Due date in the past
                        submissionDate = pastDate, // Submission date in the past
                        duration = random.Next(30, 120), // Random duration between 30 and 120 minutes
                        score = random.NextDouble() * 100, // Random score between 0 and 100
                        topicScores = new TopicScores()
                    };

                    // Set topicScores based on assessmentId
                    if (assessment.assessmentId == 1) // Math assessment
                    {
                        assessment.title = "math";
                        assessment.topicScores.math = new MathScores();

                        // Create new instances of Score for each math topic
                        assessment.topicScores.math.algebra = new Score();
                        assessment.topicScores.math.addition = new Score();
                        assessment.topicScores.math.subtraction = new Score();
                        assessment.topicScores.math.multiplication = new Score();
                        assessment.topicScores.math.division = new Score();
                        assessment.topicScores.math.fractions = new Score();
                        assessment.topicScores.math.percentages = new Score();
                        assessment.topicScores.math.decimals = new Score();
                        assessment.topicScores.math.ratio_and_proportion = new Score();
                        assessment.topicScores.math.number_and_place_value = new Score();
                        assessment.topicScores.math.measurement = new Score();
                        assessment.topicScores.math.statistics = new Score();
                        

                        // Set random scores for math topics
                        assessment.topicScores.math.algebra.total = random.Next(10, 20);
                        assessment.topicScores.math.algebra.correct = random.Next(0, (int)assessment.topicScores.math.algebra.total + 1);
                        assessment.topicScores.math.algebra.score = (assessment.topicScores.math.algebra.correct / assessment.topicScores.math.algebra.total) * 100;

                        assessment.topicScores.math.addition.total = random.Next(10, 20);
                        assessment.topicScores.math.addition.correct = random.Next(0, (int)assessment.topicScores.math.addition.total + 1);
                        assessment.topicScores.math.addition.score = (assessment.topicScores.math.addition.correct / assessment.topicScores.math.addition.total) * 100;

                        assessment.topicScores.math.subtraction.total = random.Next(10, 20);
                        assessment.topicScores.math.subtraction.correct = random.Next(0, (int)assessment.topicScores.math.subtraction.total + 1);
                        assessment.topicScores.math.subtraction.score = (assessment.topicScores.math.subtraction.correct / assessment.topicScores.math.subtraction.total) * 100;

                        assessment.topicScores.math.multiplication.total = random.Next(10, 20);
                        assessment.topicScores.math.multiplication.correct = random.Next(0, (int)assessment.topicScores.math.multiplication.total + 1);
                        assessment.topicScores.math.multiplication.score = (assessment.topicScores.math.multiplication.correct / assessment.topicScores.math.multiplication.total) * 100;

                        assessment.topicScores.math.division.total = random.Next(10, 20);
                        assessment.topicScores.math.division.correct = random.Next(0, (int)assessment.topicScores.math.division.total + 1);
                        assessment.topicScores.math.division.score = (assessment.topicScores.math.division.correct / assessment.topicScores.math.division.total) * 100;

                        assessment.topicScores.math.fractions.total = random.Next(10, 20);
                        assessment.topicScores.math.fractions.correct = random.Next(0, (int)assessment.topicScores.math.fractions.total + 1);
                        assessment.topicScores.math.fractions.score = (assessment.topicScores.math.fractions.correct / assessment.topicScores.math.fractions.total) * 100;

                        assessment.topicScores.math.percentages.total = random.Next(10, 20);
                        assessment.topicScores.math.percentages.correct = random.Next(0, (int)assessment.topicScores.math.percentages.total + 1);
                        assessment.topicScores.math.percentages.score = (assessment.topicScores.math.percentages.correct / assessment.topicScores.math.percentages.total) * 100;

                        assessment.topicScores.math.decimals.total = random.Next(10, 20);
                        assessment.topicScores.math.decimals.correct = random.Next(0, (int)assessment.topicScores.math.decimals.total + 1);
                        assessment.topicScores.math.decimals.score = (assessment.topicScores.math.decimals.correct / assessment.topicScores.math.decimals.total) * 100;

                        assessment.topicScores.math.ratio_and_proportion.total = random.Next(10, 20);
                        assessment.topicScores.math.ratio_and_proportion.correct = random.Next(0, (int)assessment.topicScores.math.ratio_and_proportion.total + 1);
                        assessment.topicScores.math.ratio_and_proportion.score = (assessment.topicScores.math.ratio_and_proportion.correct / assessment.topicScores.math.ratio_and_proportion.total) * 100;

                        assessment.topicScores.math.number_and_place_value.total = random.Next(10, 20);
                        assessment.topicScores.math.number_and_place_value.correct = random.Next(0, (int)assessment.topicScores.math.number_and_place_value.total + 1);
                        assessment.topicScores.math.number_and_place_value.score = (assessment.topicScores.math.number_and_place_value.correct / assessment.topicScores.math.number_and_place_value.total) * 100;

                        assessment.topicScores.math.measurement.total = random.Next(10, 20);
                        assessment.topicScores.math.measurement.correct = random.Next(0, (int)assessment.topicScores.math.measurement.total + 1);
                        assessment.topicScores.math.measurement.score = (assessment.topicScores.math.measurement.correct / assessment.topicScores.math.measurement.total) * 100;

                        assessment.topicScores.math.statistics.total = random.Next(10, 20);
                        assessment.topicScores.math.statistics.correct = random.Next(0, (int)assessment.topicScores.math.statistics.total + 1);
                        assessment.topicScores.math.statistics.score = (assessment.topicScores.math.statistics.correct / assessment.topicScores.math.statistics.total) * 100;
                    }
                    else if (assessment.assessmentId == 2) // English assessment
                    {
                        assessment.title = "english";
                        assessment.topicScores.english = new EnglishScores();

                        // Create new instances of Score for each English topic
                        assessment.topicScores.english.grammar_and_punctuation = new Score();
                        assessment.topicScores.english.reading_and_comprehension = new Score();
                        assessment.topicScores.english.spelling_and_vocabulary = new Score();
                        assessment.topicScores.english.writing_skills = new Score();
                        assessment.topicScores.english.comprehension_and_analysis_of_literature = new Score();
                        assessment.topicScores.english.sentence_completion = new Score();
                        assessment.topicScores.english.sentence_transformation = new Score();
                        assessment.topicScores.english.literary_terms_and_concepts = new Score();
                        assessment.topicScores.english.grammar_rules_and_usage = new Score();

                        // Set random scores for English topics
                        assessment.topicScores.english.grammar_and_punctuation.total = random.Next(10, 20);
                        assessment.topicScores.english.grammar_and_punctuation.correct = random.Next(0, (int)assessment.topicScores.english.grammar_and_punctuation.total + 1);
                        assessment.topicScores.english.grammar_and_punctuation.score = (assessment.topicScores.english.grammar_and_punctuation.correct / assessment.topicScores.english.grammar_and_punctuation.total) * 100;

                        assessment.topicScores.english.reading_and_comprehension.total = random.Next(10, 20);
                        assessment.topicScores.english.reading_and_comprehension.correct = random.Next(0, (int)assessment.topicScores.english.reading_and_comprehension.total + 1);
                        assessment.topicScores.english.reading_and_comprehension.score = (assessment.topicScores.english.reading_and_comprehension.correct / assessment.topicScores.english.reading_and_comprehension.total) * 100;

                        assessment.topicScores.english.spelling_and_vocabulary.total = random.Next(10, 20);
                        assessment.topicScores.english.spelling_and_vocabulary.correct = random.Next(0, (int)assessment.topicScores.english.spelling_and_vocabulary.total + 1);
                        assessment.topicScores.english.spelling_and_vocabulary.score = (assessment.topicScores.english.spelling_and_vocabulary.correct / assessment.topicScores.english.spelling_and_vocabulary.total) * 100;

                        assessment.topicScores.english.writing_skills.total = random.Next(10, 20);
                        assessment.topicScores.english.writing_skills.correct = random.Next(0, (int)assessment.topicScores.english.writing_skills.total + 1);
                        assessment.topicScores.english.writing_skills.score = (assessment.topicScores.english.writing_skills.correct / assessment.topicScores.english.writing_skills.total) * 100;

                        assessment.topicScores.english.comprehension_and_analysis_of_literature.total = random.Next(10, 20);
                        assessment.topicScores.english.comprehension_and_analysis_of_literature.correct = random.Next(0, (int)assessment.topicScores.english.comprehension_and_analysis_of_literature.total + 1);
                        assessment.topicScores.english.comprehension_and_analysis_of_literature.score = (assessment.topicScores.english.comprehension_and_analysis_of_literature.correct / assessment.topicScores.english.comprehension_and_analysis_of_literature.total) * 100;

                        assessment.topicScores.english.sentence_completion.total = random.Next(10, 20);
                        assessment.topicScores.english.sentence_completion.correct = random.Next(0, (int)assessment.topicScores.english.sentence_completion.total + 1);
                        assessment.topicScores.english.sentence_completion.score = (assessment.topicScores.english.sentence_completion.correct / assessment.topicScores.english.sentence_completion.total) * 100;

                        assessment.topicScores.english.sentence_transformation.total = random.Next(10, 20);
                        assessment.topicScores.english.sentence_transformation.correct = random.Next(0, (int)assessment.topicScores.english.sentence_transformation.total + 1);
                        assessment.topicScores.english.sentence_transformation.score = (assessment.topicScores.english.sentence_transformation.correct / assessment.topicScores.english.sentence_transformation.total) * 100;

                        assessment.topicScores.english.literary_terms_and_concepts.total = random.Next(10, 20);
                        assessment.topicScores.english.literary_terms_and_concepts.correct = random.Next(0, (int)assessment.topicScores.english.literary_terms_and_concepts.total + 1);
                        assessment.topicScores.english.literary_terms_and_concepts.score = (assessment.topicScores.english.literary_terms_and_concepts.correct / assessment.topicScores.english.literary_terms_and_concepts.total) * 100;

                        assessment.topicScores.english.grammar_rules_and_usage.total = random.Next(10, 20);
                        assessment.topicScores.english.grammar_rules_and_usage.correct = random.Next(0, (int)assessment.topicScores.english.grammar_rules_and_usage.total + 1);
                        assessment.topicScores.english.grammar_rules_and_usage.score = (assessment.topicScores.english.grammar_rules_and_usage.correct / assessment.topicScores.english.grammar_rules_and_usage.total) * 100;
                    }

                    seedData.Add(assessment);
                }

                // Sort the seedData list by date before adding it to the database
                seedData.Sort((a, b) => DateTime.Compare(a.submissionDate ?? DateTime.MinValue, b.submissionDate ?? DateTime.MinValue));

                _dbContext.student_assessment.AddRange(seedData);

                _dbContext.SaveChanges();
            } catch(Exception ex) {
                Console.WriteLine($"GenerateSeedData caught an exception: {ex.Message}");
                throw new Exception($"GenerateSeedData failed: {ex.Message}");
            }
        
        } 
    }
}

