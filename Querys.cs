using Model;
using Npgsql;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies ;
using Microsoft.AspNetCore.Http;


namespace AccountController
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<IdentityRole> _roleManager ;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbcontext;

        public QueryController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
           // _roleManager = roleManager;
           // _httpContextAccessor = httpContextAccessor;
           _dbcontext = dbcontext;
    
        }


        [HttpGet("GetAdminAllTeachers")] 
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAdminAllTeachers() {
            
            try
            {
                var adminTeachers = await _userManager.Users.ToListAsync();
                return Ok(adminTeachers);
   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");

            }

        }

        [HttpGet("GetAdminAllStudents")] 
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAdminAllStudents() {
            
            try
            {
                var adminStudents = await _userManager.Users.ToListAsync();
                return Ok(adminStudents);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");

            }
        }

        [HttpGet("GetAdminUser")] 
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAdminUser() {
        try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    return Ok(new{user, role});
                }
                else
                {
                    Console.WriteLine($"Student with ID {userId} not found.");
                    return StatusCode(500, "An error occurred while updating, the incident has been logged");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }

        [HttpGet("GetAdminTeacher")] 
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> GetAdminTeacher() 
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _dbcontext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new 
                    {
                        u.Id,
                        u.Name,
                        u.Dob,
                        u.PhoneNumber,
                        u.SortCode,
                        u.AccountNo,
                        u.Stream,
                        u.Adjustments,
                        u.Notes,
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    Console.WriteLine($"Admin_Teacher with that ID not found.");
                    return StatusCode(500, "An error occurred while updating, the incident has been logged");
                }

                return Ok(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }

        [HttpGet("getVerifyQuestions")] 
        public async Task<IActionResult> GetVerifyQuestions() 
        {
            try
            {
                var originalQuestions = await _dbcontext.exam_questions
                    .Where(q => q.questionType == QuestionType.Original)
                    .ToListAsync();

                return Ok(originalQuestions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }
        

        [HttpGet("GetAllTeachers")] 
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllTeachers() {
            try
            {
                var allTeachers = await _dbcontext.teachers.ToListAsync();
                return Ok(allTeachers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");

            }
            
        }

        [HttpGet("GetAllStudents")] 
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllStudents() {
            try
            {
                var allStudents = await _dbcontext.students.ToListAsync();
                return Ok(allStudents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");

            }
        }

        [HttpGet("GetAllLessons")] 
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllLessons() {
            try
            {
                var allLessons = await _dbcontext.lesson_events.ToListAsync();
                return Ok(allLessons);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");

            }
        }

        [HttpGet("GetAllCals")] 
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllCals() {
            try
            {
                var allCalenderEvents = await _dbcontext.calendar_events.ToListAsync();
                return Ok(allCalenderEvents);  
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");

            }
        }
    

        [HttpGet("GetTeacher")] 
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> GetTeacher([FromServices] IHttpContextAccessor httpContextAccessor) {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var foundTeacher = await _dbcontext.teachers
                    .Where(t => t.teacherId == userId)
                    .Select(t => new 
                    {
                        t.teacherId,
                        t.name,
                        t.messages,
                        t.notes,
                        
                        students = t.students.Select(s => new 
                        {
                            s.name,
                            s.studentId,
                            s.teacherId,
                            s.messages,
                            s.stream,
                            s.notes,
                            s.stats,
                            assessments = s.assessments.Select(a => new 
                            {
                                a.id,
                                a.title,
                                a.description,
                                a.dueDate,
                                a.teacherId,
                                a.studentId,
                                a.assessmentId,
                                a.isAssigned,
                                a.isSubmitted,
                                a.isGraded,
                                a.duration,
                                a.score,
                                a.submissionDate,
                                a.topicScores,
                                a.answers
                            }).ToList(),
                            homeworkAssignments = s.homeworkAssignments.Select(h => new 
                            {
                                h.id,
                                h.title,
                                h.description,
                                h.dueDate,
                                h.teacherId,
                                h.studentId,
                                h.isAssigned,
                                h.isSubmitted,
                                h.isGraded,
                                h.grade,
                                h.aiFeedback,
                                h.teacherFeedback,
                                h.submissionDate,
                                h.submissionContent
                            }).ToList(),
                            lessonEvents = s.lessonEvents.Select(l => new 
                            {
                                l.id,
                                l.title,
                                l.dueDate,
                                l.description,
                                l.teacherId,
                                l.studentId,
                                l.links,
                                l.isAssigned,
                                l.isComplete,
                                l.completionDate
                            }).ToList(),
                            calendarEvents = s.calendarEvents.Select(c => new 
                            {
                                c.id,
                                c.title,
                                c.description,
                                c.teacherId,
                                c.studentId,
                                c.eventId,
                                c.date,
                                c.link,
                                c.status
                            }).ToList()
                        }).ToList(),


                        assessments = t.assessments.Select(a => new 
                        {
                            a.id,
                            a.title,
                            a.description,
                            a.dueDate,
                            a.teacherId,
                            a.studentId,
                            a.assessmentId,
                            a.isAssigned,
                            a.isSubmitted,
                            a.isGraded,
                            a.duration,
                            a.score,
                            a.submissionDate,
                            a.topicScores,
                            a.answers
                        }).ToList(),
                        homeworkAssignments = t.homeworkAssignments.Select(h => new 
                        {
                            h.id,
                            h.title,
                            h.description,
                            h.dueDate,
                            h.teacherId,
                            h.studentId,
                            h.isAssigned,
                            h.isSubmitted,
                            h.isGraded,
                            h.grade,
                            h.aiFeedback,
                            h.teacherFeedback,
                            h.submissionDate,
                            h.submissionContent
                        }).ToList(),
                        calendarEvents = t.calendarEvents.Select(c => new 
                        {
                            c.id,
                            c.teacherId,
                            c.studentId,
                            c.title,
                            c.description,
                            c.eventId,
                            c.date,
                            c.link,
                            c.status
                        }).ToList(),
                        lessonEvents = t.lessonEvents.Select(l => new 
                        {
                            l.id,
                            l.title,
                            l.description,
                            l.dueDate,
                            l.teacherId,
                            l.studentId,
                            l.links,
                            l.isAssigned,
                            l.isComplete,
                            l.completionDate
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (foundTeacher != null)
                {   

                    Console.WriteLine("returned teacher data");
                    return Ok(foundTeacher);
                }
                else
                {
                    Console.WriteLine($"Student with ID {userId} has correct student policy but no student record. Should be impossible. Was there an error in sign up stage 2");
                    return NotFound("Student has no student record");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"error ocured: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while retriving data, the incident has been logged");
                
            }
        }
        
        
        [HttpGet("GetStudent")] 
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetStudent()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var foundStudent = await _dbcontext.students
                    .Include(s => s.lessonEvents)
                    .Include(s => s.calendarEvents)
                    .Include(s => s.homeworkAssignments)
                    .Include(s => s.assessments)
                    .FirstOrDefaultAsync(s => s.studentId == userId);

                if (foundStudent != null)
                {
                    return Ok(foundStudent);
                }
                else
                {
                    Console.WriteLine($"Student with ID {userId} has correct student policy but no student record. Should be impossible. Was there an error in sign up stage 2");
                    return NotFound("Student has no student record");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }   

 


        [HttpGet("GetStudents")] 
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult>  GetStudents() {
            
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var studentsWithTeacherId = _dbcontext.students
                    .Where(student => student.teacherId == userId)
                    .ToListAsync();
                
                return Ok(studentsWithTeacherId);
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }

            
        }

        [HttpGet("GetAssessmentById")] 
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAssessmentById([FromQuery] int assessmentId)
        {
            try
            {
                var assessment = await _dbcontext.assessments
                .Include(a => a.questionsWithAnswers)
                .FirstOrDefaultAsync(a => a.id == assessmentId);

                if (assessment != null)
                {
                    Console.WriteLine($"Found Assessment with ID {assessmentId}:");
                    return Ok(assessment);
                }
                else
                {
                    Console.WriteLine($"Assessment with ID {assessmentId} not found.");
                    return StatusCode(500, "An error occurred while updating, the incident has been logged");
                }

            }catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }

        [HttpGet("GetAssignmentById")] 
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAssignmentById([FromQuery] int assignmentId)
        {
            try
            {
                var foundAssessmentAssignment = await _dbcontext.student_assessment_assignment
                    .FirstOrDefaultAsync(saa => saa.id == assignmentId);

                if (foundAssessmentAssignment != null)
                {
                    Console.WriteLine($"Found StudentAssessmentAssignment with ID {assignmentId}:");
                    return Ok(foundAssessmentAssignment);
                }
                else
                {
                    Console.WriteLine($"StudentAssessmentAssignment with ID {assignmentId} not found.");
                    return StatusCode(500, "An error occurred while updating, the incident has been logged");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }


        [HttpGet("checkrole")]
        [Authorize]
        public async Task<IActionResult> CheckRole([FromServices] UserManager<ApplicationUser> userManager)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return NotFound("User not found.");
                }

                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                string entrypoint = role ?? (await userManager.IsEmailConfirmedAsync(user) ? "CreateRole" : "ConfirmEmail");

                return Ok(new { entrypoint });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while updating, the incident has been logged");
            }
        }

    } 
}