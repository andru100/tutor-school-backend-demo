using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Security.Claims;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Amazon.S3.Model;
using System;
using Model;
using Ai;


namespace HomeworkUpload;

[ApiController]
[Route("api/[controller]")]
public class UploadController  : ControllerBase
{

    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _awsAccessKeyId;
    private readonly string _awsSecretAccessKey;
    private readonly string _awsBucketName;
    

    public UploadController(ApplicationDbContext dbContext, IConfiguration configuration, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _configuration = configuration;
        _awsAccessKeyId = _configuration["AWS_ACCESS_KEY_ID"];
        _awsSecretAccessKey = _configuration["AWS_SECRET_ACCESS_KEY"];
        _awsBucketName = _configuration["AWS_BUCKET_NAME"];
    }

    [HttpPost("ProfileImgUpload")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> ProfileImgUpload(HttpContext context)
    {
        var file = context.Request.Form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var foundStudent = await _dbContext.students.FirstOrDefaultAsync(s => s.studentId == userId);

        string filePath;
        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var objectKey = file.FileName;
            var bucketName = _awsBucketName;

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = objectKey,
                BucketName = _awsBucketName
            };

            var client = new AmazonS3Client(_awsAccessKeyId, _awsSecretAccessKey, RegionEndpoint.EUWest2);

            var fileTransferUtility = new TransferUtility(client);
            try
            {
                await fileTransferUtility.UploadAsync(uploadRequest);

                Console.WriteLine("File uploaded successfully");

                var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{objectKey}";

                foundStudent.profileImgUrl = fileUrl;

                await _dbContext.SaveChangesAsync(); 

                return Ok(new{fileUrl});
                
            }
            catch (Exception ex)
            {
                // Upload failed
                Console.WriteLine("File upload failed: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return BadRequest();  
            }

            return BadRequest();
        }
    }

    //TODO add security checks to make sure studentid matches homework assignment. to avoid any student uploading to records that arent theres.

    [HttpPost("HomeworkImgUpload")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> HomeworkImgUpload(HttpContext context)
    {
        var file = context.Request.Form.Files.FirstOrDefault();
        var id = int.Parse(context.Request.RouteValues["id"].ToString());

        if (file == null || file.Length == 0)
        {
            return BadRequest();
        }

        string filePath;
        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var objectKey = file.FileName;
            var bucketName = _awsBucketName;

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = objectKey,
                BucketName = _awsBucketName
            };

            var client = new AmazonS3Client(_awsAccessKeyId, _awsSecretAccessKey, RegionEndpoint.EUWest2);

            var fileTransferUtility = new TransferUtility(client);
            try
            {
                await fileTransferUtility.UploadAsync(uploadRequest);

                Console.WriteLine("File uploaded successfully");

                var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{objectKey}";

                var homework = await _dbContext.homework_assignments.FindAsync(id);

                if (homework != null)
                {
                    if (IsAiHomeworkFeedbackEnabled()) // Check if AiHomeworkFeedback is enabled
                    {
                        var img2txt = new TxtExtractor(_httpClient, _configuration);
                        var text = await img2txt.AnalyseImage(fileUrl);

                        var stream = homework.stream.ToString();
                        var HomeworkInput = new HomeworkFeedbackInput { instructions = homework.description, submission = text, stream = stream };

                        
                        var aiTeacher = new Assistant(_httpClient, _configuration);
                        var aiFeedback = await aiTeacher.HomeworkFeedbackAgent(HomeworkInput);
                        Console.WriteLine("feedback is: " + aiFeedback);

                        homework.isSubmitted = true;
                        homework.submissionContentType = SubmissionContentType.Image;
                        homework.submissionContent = fileUrl;
                        homework.aiFeedback = aiFeedback;
                        homework.dueDate = DateTime.UtcNow;
                        homework.submissionDate = DateTime.UtcNow;
                        _dbContext.Update(homework);
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine("finished uploading sending 200");
                        return Ok();
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
                        Console.WriteLine("finished uploading sending 200");
                        return Ok();
                    }
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
                return BadRequest();  
            }

            return BadRequest();
        }
    }


    [HttpPost("HomeworkDocumentUpload")] 
    [Authorize(Policy = "Student")]
    public async Task<IActionResult> HomeworkDocumentUpload(HttpContext context)
    {
        var file = context.Request.Form.Files.FirstOrDefault();
        var id = int.Parse(context.Request.RouteValues["id"].ToString());

        if (file == null || file.Length == 0)
        {
            return BadRequest();
        }

        string filePath;
        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var objectKey = file.FileName;
            var bucketName = _awsBucketName;

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = objectKey,
                BucketName = _awsBucketName
            };

            var client = new AmazonS3Client(_awsAccessKeyId, _awsSecretAccessKey, RegionEndpoint.EUWest2);

            var fileTransferUtility = new TransferUtility(client);
            try
            {
                await fileTransferUtility.UploadAsync(uploadRequest);

                Console.WriteLine("File uploaded successfully");

                var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{objectKey}";

                var homework = await _dbContext.homework_assignments.FindAsync(id);

                if (homework != null)
                {
                    if (IsAiHomeworkFeedbackEnabled()) // Check if AiHomeworkFeedback is enabled
                    {
                        var doc2txt = new TxtExtractor(_httpClient, _configuration) ;
                        var text = await doc2txt.AnalyseDocument(fileUrl);

                        var stream = homework.stream.ToString();
                        var HomeworkInput = new HomeworkFeedbackInput { instructions = homework.description, submission = text, stream = stream };

                        
                        var aiTeacher = new Assistant(_httpClient, _configuration);
                        var aiFeedback = await aiTeacher.HomeworkFeedbackAgent(HomeworkInput);
                        Console.WriteLine("feedback is: " + aiFeedback);

                        homework.isSubmitted = true;
                        homework.submissionContentType = SubmissionContentType.Image;
                        homework.submissionContent = fileUrl;
                        homework.aiFeedback = aiFeedback;
                        homework.dueDate = DateTime.UtcNow;
                        homework.submissionDate = DateTime.UtcNow;
                        _dbContext.Update(homework);
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine("finished uploading sending 200");
                        return Ok();
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
                        Console.WriteLine("finished uploading sending 200");
                        return Ok();
                    }
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
                return BadRequest();  
            }

            return BadRequest();
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
                var homework = await _dbContext.homework_assignments.FindAsync(homeworkUploadData.id);

                if (IsAiHomeworkFeedbackEnabled()) // Check if AiHomeworkFeedback is enabled
                {
                    var aiTeacher = new Assistant(_httpClient, _configuration);
                    
                    var stream = homework.stream.ToString();
                    var HomeworkInput = new HomeworkFeedbackInput { instructions = homework.description, submission = homeworkUploadData.text, stream = stream };
                    var aiFeedback = await aiTeacher.HomeworkFeedbackAgent(HomeworkInput);
                    Console.WriteLine("feedback is: " + aiFeedback);

                    homework.isSubmitted = true;
                    homework.submissionContentType = SubmissionContentType.Text;
                    homework.submissionContent = homeworkUploadData.text;
                    homework.aiFeedback = aiFeedback;
                    homework.dueDate = DateTime.UtcNow;
                    homework.submissionDate = DateTime.UtcNow;

                    _dbContext.Update(homework);
                    await _dbContext.SaveChangesAsync();
                    Console.WriteLine("finished uploading sending 200");
                    return Ok();
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
                    Console.WriteLine("finished uploading sending 200");
                    return Ok();
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