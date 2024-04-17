using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Model;

namespace AWSEmail;

//semds email, implements Identity IEmailSender interface
public class AWSEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IConfiguration _configuration;
    private readonly string _awsAccessKeyId;
    private readonly string _awsSecretAccessKey;
    private readonly string _senderEmail;

    public AWSEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
        _awsAccessKeyId = _configuration["AWS_ACCESS_KEY_ID"];
        _awsSecretAccessKey = _configuration["AWS_SECRET_ACCESS_KEY"];
        _senderEmail = _configuration["AWS_SENDER_EMAIL"];
    }


    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using (var client = new AmazonSimpleEmailServiceClient(_awsAccessKeyId, _awsSecretAccessKey, RegionEndpoint.EUWest1))
        {
            var sendRequest = new SendEmailRequest
            {
                Source = _senderEmail,
                Destination = new Destination { ToAddresses = new List<string> { email } },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body { Text = new Content(message) }
                }
            };

            try
            {
                await client.SendEmailAsync(sendRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email could not be sent: " + ex.Message);
            }
        }
    }

       public async Task SendConfirmationLinkAsync(ApplicationUser user, string link, string callbackUrl)
    {
        Console.WriteLine("SendConfirmationLinkAsync triggered \n\n reset link is " + link );
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string link, string callbackUrl)
    {
        Console.WriteLine("SendPasswordResetLinkAsync triggered \n\n reset link is " + link );
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string code, string callbackUrl)
    {
        Console.WriteLine("sendPasswordReset triggered \n\n reset code is " + code);
    }
}
