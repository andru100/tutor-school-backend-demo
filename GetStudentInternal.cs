using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model; 
using System;
using System.Threading.Tasks;

namespace GetStudent;

public class GetStudentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetStudentService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Student> GetStudentByIdAsync(string studentId)
    {
        try
        {
            var foundStudent = await _dbContext.students
                .Include(s => s.lessonEvents)
                .Include(s => s.calendarEvents)
                .Include(s => s.homeworkAssignments)
                .Include(s => s.assessments)
                .FirstOrDefaultAsync(s => s.studentId == studentId);

            if (foundStudent == null)
            {
                Console.WriteLine($"Student with ID {studentId} not found.");
                throw new Exception("Student not found");
            }

            return foundStudent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            throw; 
        }
    }
}