using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;

using AccountController; 
using Amazon.S3.Model;
using System;
using Model;
using Ai;
using GetStudent;




namespace HomeworkUpload;

[ApiController]
[Route("api/[controller]")]
public class UploadController  : ControllerBase
{

    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string _azureBlobConnectionString;
    private readonly string _azureBlobContainerName;
    private readonly GetStudentService _getStudentService;


    public UploadController(ApplicationDbContext dbContext, IConfiguration configuration, HttpClient httpClient, UserManager<ApplicationUser> userManager, GetStudentService getStudentService)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _configuration = configuration;
        _userManager = userManager;
        _getStudentService = getStudentService;
    }

    [HttpPost("ProfileImgUpload")]
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> ProfileImgUpload()
    {
        Console.WriteLine("\n\nprofil img upload called\n\n");
        var file = HttpContext.Request.Form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest("User ID not found.");
        }

        // Check the user's role
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        if (userRole == null)
        {
            return BadRequest("User role not found.");
        }

        // Find the corresponding record based on the role
        dynamic foundUser = null;
        if (userRole == "Student")
        {
            foundUser = await _dbContext.students.FirstOrDefaultAsync(s => s.studentId == userId);
        }
        else if (userRole == "Teacher")
        {
            foundUser = await _dbContext.teachers.FirstOrDefaultAsync(t => t.teacherId == userId);
        }

        if (foundUser == null)
        {
            return BadRequest($"{userRole} not found.");
        }

        string filePath;
        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var storageAccountName = _configuration["AzureBlobStorage:AccountName"];
            var containerName = _configuration["AzureBlobStorage:ContainerName"];

            if (string.IsNullOrEmpty(storageAccountName) || string.IsNullOrEmpty(containerName))
            {
                return BadRequest("Azure storage account name or container name is not configured.");
            }

            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                new DefaultAzureCredential()
            );

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(file.FileName);

            try
            {
                newMemoryStream.Position = 0;
                await blobClient.UploadAsync(newMemoryStream, new BlobHttpHeaders { ContentType = file.ContentType });

                Console.WriteLine("File uploaded successfully");

                var fileUrl = blobClient.Uri.ToString();

                foundUser.profileImgUrl = fileUrl;

                await _dbContext.SaveChangesAsync();

                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                // Upload failed
                Console.WriteLine("File upload failed: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return BadRequest("File upload failed.");
            }
        }
    }


    [HttpPost("HomeworkImgUpload/{id}")]
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> HomeworkImgUpload(int id)
    {
        Console.WriteLine("\n\nhomework img upload called\n\n");
        var file = HttpContext.Request.Form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var homework = await _dbContext.homework_assignments.FindAsync(id);
        if (homework == null)
        {
            return BadRequest("Homework assignment not found.");
        }

        string filePath;
        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var storageAccountName = _configuration["AzureBlobStorage:AccountName"];
            var containerName = _configuration["AzureBlobStorage:ContainerName"];

            if (string.IsNullOrEmpty(storageAccountName) || string.IsNullOrEmpty(containerName))
            {
                return BadRequest("Azure storage account name or container name is not configured.");
            }

            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                new DefaultAzureCredential()
            );

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(file.FileName);

            try
            {
                newMemoryStream.Position = 0;
                await blobClient.UploadAsync(newMemoryStream, new BlobHttpHeaders { ContentType = file.ContentType });

                Console.WriteLine("AAAAAFile uploaded successfully");

                var fileUrl = blobClient.Uri.ToString();

                if (IsAiHomeworkFeedbackEnabled()) 
                {
                    var img2txt = new TxtExtractor(_httpClient, _configuration);
                    var text = await img2txt.AnalyseImage(fileUrl);

                    var stream = homework.stream.ToString();
                    var HomeworkInput = new HomeworkFeedbackInput { instructions = homework.description, submission = text, stream = stream };
                    Console.WriteLine("calling homework feedback agent");
                    var aiTeacher = new Assistant(_httpClient, _configuration);
                    var aiFeedback = await aiTeacher.HomeworkFeedbackAgent(HomeworkInput);

                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Image;
                    homework.submissionContent = fileUrl;
                    homework.aiFeedback = aiFeedback;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;
                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    var student = await _getStudentService.GetStudentByIdAsync(homework.studentId);
                    return Ok(student);
                }
                else
                {
                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Image;
                    homework.submissionContent = fileUrl;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;
                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    var student = await _getStudentService.GetStudentByIdAsync(homework.studentId);
                    return Ok(student);
                }
            }
            catch (Exception ex)
            {
                // Upload failed
                Console.WriteLine("An error occured during homework image upload: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return BadRequest("File upload failed.");
            }
        }
    }


    [HttpPost("HomeworkDocumentUpload")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> HomeworkDocumentUpload(HttpContext context)
    {
        Console.WriteLine("\n\nhomework document upload called\n\n");
        var file = context.Request.Form.Files.FirstOrDefault();
        var id = int.Parse(context.Request.RouteValues["id"].ToString());

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest("User ID not found.");
        }

        var homework = await _dbContext.homework_assignments.FindAsync(id);
        if (homework == null)
        {
            return BadRequest("Homework assignment not found.");
        }

        // Check if the user is authorized to upload for this homework assignment
        if (homework.studentId != userId && homework.teacherId != userId)
        {
            return Forbid("You are not authorized to upload for this homework assignment.");
        }

        string filePath;
        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var storageAccountName = _configuration["AzureBlobStorage:AccountName"];
            var containerName = _configuration["AzureBlobStorage:ContainerName"];

            if (string.IsNullOrEmpty(storageAccountName) || string.IsNullOrEmpty(containerName))
            {
                return BadRequest("Azure storage account name or container name is not configured.");
            }

            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                new DefaultAzureCredential()
            );

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(file.FileName);

            try
            {
                newMemoryStream.Position = 0;
                await blobClient.UploadAsync(newMemoryStream, new BlobHttpHeaders { ContentType = file.ContentType });

                Console.WriteLine("File uploaded successfully");

                var fileUrl = blobClient.Uri.ToString();

                if (IsAiHomeworkFeedbackEnabled()) 
                {
                    var doc2txt = new TxtExtractor(_httpClient, _configuration);
                    var text = await doc2txt.AnalyseDocument(fileUrl);

                    var stream = homework.stream.ToString();
                    var HomeworkInput = new HomeworkFeedbackInput { instructions = homework.description, submission = text, stream = stream };

                                    var aiTeacher = new Assistant(_httpClient, _configuration);
                    var aiFeedback = await aiTeacher.HomeworkFeedbackAgent(HomeworkInput);

                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Document;
                    homework.submissionContent = fileUrl;
                    homework.aiFeedback = aiFeedback;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;
                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    var student = await _getStudentService.GetStudentByIdAsync(homework.studentId);
                    return Ok(student);
                }
                else
                {
                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Document;
                    homework.submissionContent = fileUrl;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;
                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    var student = await _getStudentService.GetStudentByIdAsync(homework.studentId);
                    return Ok(student);
                }
            }
            catch (Exception ex)
            {
                // Upload failed
                Console.WriteLine("File upload failed: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return BadRequest("File upload failed.");
            }
        }
    }

    [HttpPost("HomeworkTxtUpload")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> HomeworkTxtUpload(HomeworkUploadTxtData homeworkUploadData)
    {
        try
        {
            if (homeworkUploadData != null)
            {
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return BadRequest("User ID not found.");
                }

                var homework = await _dbContext.homework_assignments.FindAsync(homeworkUploadData.id);
                if (homework == null)
                {
                    return BadRequest("Homework assignment not found.");
                }

                // Check if the user is authorized to upload for this homework assignment
                if (homework.studentId != userId && homework.teacherId != userId)
                {
                    return Forbid("You are not authorized to upload for this homework assignment.");
                }

                if (IsAiHomeworkFeedbackEnabled())
                {
                    var aiTeacher = new Assistant(_httpClient, _configuration);
                    
                    var stream = homework.stream.ToString();
                    var HomeworkInput = new HomeworkFeedbackInput { instructions = homework.description, submission = homeworkUploadData.text, stream = stream };
                    var aiFeedback = await aiTeacher.HomeworkFeedbackAgent(HomeworkInput);

                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Text;
                    homework.submissionContent = homeworkUploadData.text;
                    homework.aiFeedback = aiFeedback;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;

                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    var student = await _getStudentService.GetStudentByIdAsync(homework.studentId);
                    return Ok(student);
                }
                else
                {
                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Text;
                    homework.submissionContent = homeworkUploadData.text;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;

                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    var student = await _getStudentService.GetStudentByIdAsync(homework.studentId);
                    return Ok(student);
                }
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine("File upload failed: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return BadRequest();
        }

        return BadRequest();
    }
    

    private bool IsAiHomeworkFeedbackEnabled()
    {
        // Check the flag from appsettings.json, defaults to true if not found
        bool aiFeedbackEnabled = Convert.ToBoolean(_configuration["ENABLE_AI_HOMEWORK_FEEDBACK=true"] ?? "true");
        return aiFeedbackEnabled;
    }
}