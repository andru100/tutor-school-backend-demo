using Model;
using Npgsql;
using seed;
using Ai;
using Grades;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using GetStudent;

namespace AccountController;

[ApiController]
[Route("api/[controller]")]
public class MutationController : ControllerBase
{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender<ApplicationUser> _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager ;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbcontext;
        private readonly GetStudentService _getStudentService;

        // Validate the email address 
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        public MutationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender<ApplicationUser> emailSender, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ApplicationDbContext dbcontext, GetStudentService getStudentService )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
            _getStudentService = getStudentService;
    
        }


    [HttpPost("AdminTeacherUpdate")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> AdminTeacherUpdate([FromBody] ApplicationUser infoRequest)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != infoRequest.Id)
                {
                    return BadRequest("User can only update records they own");
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (!string.IsNullOrEmpty(infoRequest.Email))
                {
                    var email = await _userManager.GetEmailAsync(user);

                    if (!_emailAddressAttribute.IsValid(infoRequest.Email))
                    {
                        return BadRequest("invalid email");
                    }

                    if (email != infoRequest.Email)
                    {
                        var AccountClasss = new AccountController(_userManager, _signInManager, _configuration, _dbcontext);
                        await AccountClasss.ConfirmationEmailAsync(user, httpContext, infoRequest.Email, isChange: true);
                    }
                }

                if (!string.IsNullOrEmpty(infoRequest.Name))
                {
                    user.Name = infoRequest.Name; 
                }
                if (infoRequest.Dob.HasValue)
                {
                    user.Dob = infoRequest.Dob.Value;
                }
                if (!string.IsNullOrEmpty(infoRequest.PhoneNumber))
                {
                    user.PhoneNumber = infoRequest.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(infoRequest.SortCode))
                {
                    user.SortCode = infoRequest.SortCode;
                }
                if (!string.IsNullOrEmpty(infoRequest.AccountNo))
                {
                    user.AccountNo = infoRequest.AccountNo;
                }
                if (!string.IsNullOrEmpty(infoRequest.Stream))
                {
                    user.Stream = infoRequest.Stream;
                }
                if (!string.IsNullOrEmpty(infoRequest.Notes))
                {
                    user.Notes = infoRequest.Notes;
                } 
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrEmpty(infoRequest.Name))
                {
                    var teacher = await _dbcontext.teachers.FirstOrDefaultAsync(t => t.teacherId == userId);
                    if (teacher != null)
                    {
                        teacher.name = infoRequest.Name;
                        await _dbcontext.SaveChangesAsync();
                    }
                }

                transaction.Commit();

                return Ok(user);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while updating admin teacher: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating admin teacher, the incident has been logged");
            }
        }
    }

    [HttpPost("AdminStudentUpdate")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> AdminStudentUpdate(ApplicationUser input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != input.Id)
                {
                    return BadRequest("User can only update recors they own");
                }

                var user = await _userManager.FindByIdAsync(userId);
                
                if (!string.IsNullOrEmpty(input.Email))
                {
                    var email = await _userManager.GetEmailAsync(user);

                    if (email != input.Email)
                    {
                        var AccountClasss = new AccountController(_userManager, _signInManager, _configuration, _dbcontext);
                        await AccountClasss.ConfirmationEmailAsync(user, httpContext, input.Email, isChange: true);
                    }
                }
                if (!string.IsNullOrEmpty(input.Name))
                {
                    user.Name = input.Name; 
                }
                if (input.Dob.HasValue)
                {
                    user.Dob = input.Dob.Value;
                }
                if (!string.IsNullOrEmpty(input.PhoneNumber))
                {
                    user.PhoneNumber = input.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(input.SortCode))
                {
                    user.SortCode = input.SortCode;
                }
                if (!string.IsNullOrEmpty(input.AccountNo))
                {
                    user.AccountNo = input.AccountNo;
                }
                if (!string.IsNullOrEmpty(input.Stream))
                {
                    user.Stream = input.Stream;
                }
                if (!string.IsNullOrEmpty(input.Notes))
                {
                    user.Notes = input.Notes;
                }
                if (!string.IsNullOrEmpty(input.ParentName))
                {
                    user.ParentName = input.ParentName;
                }
                await _userManager.UpdateAsync(user);

                var student = await _dbcontext.students.FirstOrDefaultAsync(t => t.studentId == userId);
                if (!string.IsNullOrEmpty(input.Name))
                {
                    student.name = input.Name;
                }
                if (!string.IsNullOrEmpty(input.Stream))
                {
                    student.stream = input.Stream;   
                }
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                return Ok(user);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while updating admin student: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating the student details, the incident has been logged");
            }
        }
    }

    [HttpPost("AdminDeleteTeacher")] 
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> AdminDeleteTeacher(AdminDelete input) 
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                DeleteTeacher(input);
                var user = await _userManager.FindByIdAsync(input.id);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        transaction.Commit();
                        return Ok(new { message = "user successfully deleted" });
                    } 
                    else 
                    {
                        transaction.Rollback();
                        return BadRequest("bad request unable to delete");
                    }
                } 
                else 
                {
                    transaction.Rollback();
                    return BadRequest("unable to delete user");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while deleting the admin teacher record: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while deleting the teacher details, the incident has been logged");
            }
        }
    }

    [HttpPost("AdminDeleteStudent")] 
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> AdminDeleteStudent(AdminDelete input) 
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                DeleteStudent(input);
                var user = await _userManager.FindByIdAsync(input.id);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        transaction.Commit();
                        return Ok(new { message = "user successfully deleted" });
                    } 
                    else 
                    {
                        transaction.Rollback();
                        return BadRequest("bad request unable to delete");
                    }
                } 
                else 
                {
                    transaction.Rollback();
                    return BadRequest("unable to delete user");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while deleting the admin students record: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the students record, the incident has been logged");
            }
        }
    }

    [HttpPost("DeleteTeacher")] 
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteTeacher(AdminDelete input) {
        
        try
        {

            var teacherToDelete = _dbcontext.teachers.Find(input.id);
            _dbcontext.teachers.Remove(teacherToDelete);
            _dbcontext.SaveChanges();

            return Ok(new { message = "user successfully deleted" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting the teacher record: {ex.Message}");
            if (ex.InnerException != null)
            {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while deleting the teacher record, the incident has been logged");
        }
        
    }

    [HttpPost("DeleteStudent")] 
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteStudent(AdminDelete input) {
        
        try
        {

            var studentToDelete = _dbcontext.students.Find(input.id);
            _dbcontext.students.Remove(studentToDelete);
            _dbcontext.SaveChanges();

            return Ok(new { message = "user successfully deleted" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting the admin students record: {ex.Message}");
            if (ex.InnerException != null)
            {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while deleting the admin students record, the incident has been logged");
        }
        
    }

    [HttpPost("AddLessonEvent")]
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> AddLessonEvent([FromBody] LessonEvent input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                input.dueDate = DateTime.SpecifyKind(input.dueDate, DateTimeKind.Utc);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                input.teacherId = userId;
                _dbcontext.lesson_events.Add(input);
                await _dbcontext.SaveChangesAsync();
                var lessonId = input.id ?? 0;
                if (lessonId == 0)
                {
                    transaction.Rollback();
                    return BadRequest("Lesson ID cannot be null");
                }

                var CalendarEvent = new CalendarEvent
                {
                    eventId = lessonId,
                    studentId = input.studentId,
                    teacherId = userId,
                    title = input.title,
                    description = input.description,
                    link = input.links,
                    status = "pending",
                    date = input.dueDate
                };
                _dbcontext.calendar_events.Add(CalendarEvent);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                var foundStudent = await _getStudentService.GetStudentByIdAsync(input.studentId);
                return Ok(foundStudent);
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Console.WriteLine($"An error occurred while creating the lesson event: {ex.Message}");
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }

    [HttpPost("UpdateLessonEvent")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> UpdateLessonEvent(LessonEvent input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != input.teacherId)
                {
                    return BadRequest("User Id does not match input teacherId the lesson is assigned to");
                }

                input.dueDate = DateTime.SpecifyKind(input.dueDate, DateTimeKind.Utc);
                _dbcontext.lesson_events.Update(input);
                await _dbcontext.SaveChangesAsync();

                var CalendarEvent = _dbcontext.calendar_events.FirstOrDefault(e => e.eventId == input.id);
                if (CalendarEvent != null)
                {
                    CalendarEvent.title = input.title;
                    CalendarEvent.description = input.description;
                    CalendarEvent.link = input.links;
                    CalendarEvent.date = input.dueDate;

                    _dbcontext.calendar_events.Update(CalendarEvent);
                    await _dbcontext.SaveChangesAsync();
                }

                transaction.Commit();

                
                var updated = await _getStudentService.GetStudentByIdAsync(input.studentId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while updating the lesson event: {ex.Message}");
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }

    [HttpPost("AddHomework")]  
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> AddHomework(HomeworkAssignment input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {   
                input.dueDate = DateTime.SpecifyKind(input.dueDate, DateTimeKind.Utc);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                input.teacherId = userId;
                
                _dbcontext.homework_assignments.Add(input);
                await _dbcontext.SaveChangesAsync();

                var homeworkId = input.id ?? 0;

                if (homeworkId == 0)
                {
                    return BadRequest("Lesson ID cannot be null");
                }

                var CalendarEvent = new CalendarEvent
                {
                    eventId = homeworkId,
                    studentId = input.studentId,
                    teacherId = userId, 
                    title = input.title,
                    description = input.description,
                    status = "pending",
                    date = input.dueDate
                };
                
                _dbcontext.calendar_events.Add(CalendarEvent);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                
                var updated = await _getStudentService.GetStudentByIdAsync(input.studentId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while creating the homework event: {ex.Message}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }


    [HttpPost("UpdateHomework")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> UpdateHomework(HomeworkAssignment input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {   
                if (input.submissionDate.HasValue)
                {
                    input.submissionDate = DateTime.SpecifyKind(input.submissionDate.Value, DateTimeKind.Utc);
                }
                
                input.dueDate = DateTime.SpecifyKind(input.dueDate, DateTimeKind.Utc);
                
                _dbcontext.homework_assignments.Update(input);
                await _dbcontext.SaveChangesAsync();

                var CalendarEvent = _dbcontext.calendar_events.FirstOrDefault(e => e.eventId == input.id);
                if (CalendarEvent != null)
                {
                    CalendarEvent.title = input.title;
                    CalendarEvent.description = input.description;
                    CalendarEvent.date = input.dueDate;
                    
                    _dbcontext.calendar_events.Update(CalendarEvent);
                    await _dbcontext.SaveChangesAsync();
                }

                transaction.Commit();

                
                var updated = await _getStudentService.GetStudentByIdAsync(input.studentId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while updating the homework event: {ex.Message}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }


    [HttpPost("AssignAssessment")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> AssignAssessment(StudentAssessmentAssignment input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                input.dueDate = DateTime.SpecifyKind(input.dueDate, DateTimeKind.Utc);
                string title = null;

                switch (input.assessmentId)
                {
                    case 1:
                        title = "math";
                        break;
                    case 2:
                        title = "english";
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentException("Invalid assessment ID");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                input.teacherId = userId;

                var assignment = new StudentAssessmentAssignment
                {
                    studentId = input.studentId,
                    teacherId = userId,
                    assessmentId = input.assessmentId,
                    isAssigned = input.isAssigned,
                    isSubmitted = input.isSubmitted,
                    isGraded = input.isGraded,
                    dueDate = input.dueDate,
                    title = title
                };

                _dbcontext.student_assessment_assignment.Add(assignment);
                await _dbcontext.SaveChangesAsync();

                var assignmentId = assignment.id ?? 0; 

                if (assignmentId == 0)
                {
                    return BadRequest("Lesson ID cannot be null");
                }

                var CalendarEvent = new CalendarEvent
                {
                    eventId = assignmentId,
                    studentId = input.studentId,
                    teacherId = userId, 
                    title = title,
                    description = "formal assessment",
                    status = "pending",
                    date = input.dueDate
                };
                _dbcontext.calendar_events.Add(CalendarEvent);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                
                var updated = await _getStudentService.GetStudentByIdAsync(input.studentId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while creating the assessment: {ex.Message}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }


    [HttpPost("UpdateAssignment")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> UpdateAssignment(StudentAssessmentAssignment input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                input.dueDate = DateTime.SpecifyKind(input.dueDate, DateTimeKind.Utc);
                string stream = null;

                switch (input.assessmentId)
                {
                    case 0:
                        stream = "Select";
                        break;
                    case 1:
                        stream = "math";
                        break;
                    case 2:
                        stream = "english";
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(stream))
                {
                    throw new ArgumentException("Invalid assessment ID");
                }

                input.title = stream;
                
                _dbcontext.student_assessment_assignment.Update(input);
                await _dbcontext.SaveChangesAsync();

                var CalendarEvent = _dbcontext.calendar_events.FirstOrDefault(e => e.eventId == input.id);
                if (CalendarEvent != null)
                {
                    CalendarEvent.title = stream;
                    CalendarEvent.date = input.dueDate;
                    _dbcontext.calendar_events.Update(CalendarEvent);
                    await _dbcontext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Error occurred during generation");
                }

                transaction.Commit();

                
                var updated = await _getStudentService.GetStudentByIdAsync(input.studentId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while updating the assignment: {ex.Message}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }


    [HttpPost("SubmitAssessment")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> SubmitAssessment(StudentAssessmentAssignment input)
    {
        try
        {
            var queryInstance = new QueryController(_userManager, _signInManager, _dbcontext);
            var assessmentResult = await queryInstance.GetAssessmentById(input.assessmentId);
            var assessment = (Assessment)((ObjectResult)assessmentResult).Value;
            var grader = new AssessmentGrader();
            var gradedAssignment = grader.GradeAssessment(input, assessment);

            _dbcontext.student_assessment_assignment.Update(gradedAssignment);
            _dbcontext.SaveChanges();

            var updatedStudent = await _getStudentService.GetStudentByIdAsync(input.studentId);

            return Ok(new { gradedAssignment, updatedStudent });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while assigning assessment: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while updating, the incident has been logged");
        }
    }


    [HttpPost("DeleteLesson")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> DeleteLesson(LessonDelete input) 
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var calendarEventToDelete = _dbcontext.calendar_events.FirstOrDefault(e => e.eventId == input.id);
                if (calendarEventToDelete == null)
                {
                    throw new Exception("Calendar event not found");
                }

                _dbcontext.calendar_events.Remove(calendarEventToDelete);
                await _dbcontext.SaveChangesAsync();
                
                var lessonToDelete = _dbcontext.lesson_events.Find(input.id);
                if (lessonToDelete == null)
                {
                    throw new Exception("Lesson event not found");
                }
                
                _dbcontext.lesson_events.Remove(lessonToDelete);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                return Ok(new { message = "lesson successfully deleted"});
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while deleting the lesson: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }

    [HttpPost("DeleteHomeworkAssignment")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> DeleteHomeworkAssignment(HomeworkDelete input) 
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var calendarEventToDelete = _dbcontext.calendar_events.FirstOrDefault(e => e.eventId == input.id);
                if (calendarEventToDelete != null)
                {
                    _dbcontext.calendar_events.Remove(calendarEventToDelete);
                    await _dbcontext.SaveChangesAsync();
                }
                var homeworkAssignmentToDelete = _dbcontext.homework_assignments.Find(input.id);
                _dbcontext.homework_assignments.Remove(homeworkAssignmentToDelete);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                return Ok(new { message = "homework successfully deleted" });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while deleting the homework assignment: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }

    [HttpPost("DeleteCalendarEvent")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> DeleteCalendarEvent(CalendarDelete input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var calendarEventToDelete = _dbcontext.calendar_events.Find(input.id);
                _dbcontext.calendar_events.Remove(calendarEventToDelete);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                return Ok(new { message = "deleted successfully!" });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while deleting the assessment assignment: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }

    
    [HttpPost("DeleteAssessmentAssignment")] 
    [Authorize(Policy = "Teacher")]
    public async Task<IActionResult> DeleteAssessmentAssignment(AssignmentDelete input)
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            try
            {
                var calendarEventToDelete = _dbcontext.calendar_events.FirstOrDefault(e => e.eventId == input.id);
                if (calendarEventToDelete != null)
                {
                    _dbcontext.calendar_events.Remove(calendarEventToDelete);
                    await _dbcontext.SaveChangesAsync();
                }
                var assignmentToDelete = _dbcontext.student_assessment_assignment.Find(input.id);
                _dbcontext.student_assessment_assignment.Remove(assignmentToDelete);
                await _dbcontext.SaveChangesAsync();

                transaction.Commit();

                return Ok(new { message = "deleted successfully!" });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"An error occurred while deleting the assessment assignment: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
    }



    [HttpPost("seed")] 
    public async Task<IActionResult> SeedTestData([FromServices] IUserStore<ApplicationUser> userStore, [FromServices] IServiceProvider sp)
    {
        try
        {  
            
            Console.WriteLine("attempting seed");
            var go = new Seed(_userManager, _roleManager, userStore, _dbcontext, _configuration);
            await go.SeedData(sp) ;
            return Ok(new { message = "Seed data successfully added" });
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while seeding: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while updating, the incident has been logged");
        }
    }

    [HttpPost("resetdb")]
    public IActionResult ResetDatabase([FromServices] ApplicationDbContext _dbcontext)
    {
        try
        {
            _dbcontext.Database.EnsureDeleted();
            _dbcontext.Database.EnsureCreated();
            return Ok(new { message = "Database reset and recreated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Database reset failed: {ex.Message}" });
        }
    }

    [HttpPost("CreateAssessment")]
    public async Task<IActionResult> CreateAssessment([FromBody] SeedAssessmentRequest request)
    {
        try
        {
            if (request.teacherId == null)
            {
                Console.WriteLine("teacherId is null");
                return BadRequest();
            }

            Console.WriteLine($"Seeding {request.assessmentInfo.title} for teacherId: {request.teacherId}");

            var assessment = new Assessment
            {
                title = request.assessmentInfo.title,
                stream = request.assessmentInfo.stream
            };

            _dbcontext.assessments.Add(assessment);
            await _dbcontext.SaveChangesAsync();

            int assessmentId = assessment.id;

            // Assign the assessmentId to each question and add them to the database
            foreach (var question in request.questions)
            {
                question.assessmentId = assessmentId;
                _dbcontext.exam_questions.Add(question);
            }
            await _dbcontext.SaveChangesAsync(); 

            var studentAssessmentAssignment = new StudentAssessmentAssignment
            {
                title = assessment.title.ToLower(),
                studentId = request.studentId,
                teacherId = request.teacherId,
                assessmentId = assessmentId,
                isAssigned = true,
                isSubmitted = false,
                isGraded = false,
                dueDate = new DateTime(2023, new Random().Next(1, 13), new Random().Next(1, 29), 0, 0, 0, DateTimeKind.Utc),
                score = null
            };

            _dbcontext.student_assessment_assignment.Add(studentAssessmentAssignment);
            await _dbcontext.SaveChangesAsync(); 

            return Ok();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the assessment: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the assessment" });
        }
    }
 
}
