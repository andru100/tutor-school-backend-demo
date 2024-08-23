using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Model;
using HomeworkUpload;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AWSEmail;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;


namespace AccountController 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbcontext;

        private static readonly EmailAddressAttribute _emailAddressAttribute = new();
        
        private string confirmEmailEndpointName = "ConfirmEmailRoute";
         

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext dbcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbcontext = dbcontext;
    
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SignUpData registration, [FromServices] IServiceProvider sp, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

                if (!userManager.SupportsUserEmail)
                {
                    throw new NotSupportedException($"{nameof(AccountController)} requires a user store with email support.");
                }

                var userStore = sp.GetRequiredService<IUserStore<ApplicationUser>>();
                var emailStore = (IUserEmailStore<ApplicationUser>)userStore;
                var email = registration.Email;

                if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
                {
                    return BadRequest(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
                }

                var user = new ApplicationUser();
                await userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, email, CancellationToken.None);
                var result = await userManager.CreateAsync(user, registration.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }

                await transaction.CommitAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"An error occurred during registration: {ex.Message}");
                return Problem("An error occurred during registration.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("registerGoogle")]
        public async Task<IActionResult> RegisterGoogle([FromBody] GoogleTokenData registration, [FromServices] IServiceProvider sp, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
        {
            var clientId = _configuration["GOOGLE_CLIENT_ID"];
            var clientSecret = _configuration["GOOGLE_CLIENT_SECRET"];
            var redirectUri = _configuration["GOOGLE_REDIRECT_URI"];
            var oauthApiAddress = _configuration["GOOGLE_OAUTH_API_ADDRESS"];

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { clientId }
            };

            try
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, oauthApiAddress);

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "code", registration.Credential },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                });

                var response = await httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);

                var idToken = responseObject.id_token.ToString();
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                if (payload == null)
                {
                    return Problem("Invalid Google auth token");
                }

                var Password = "Password123!";

                var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

                if (!userManager.SupportsUserEmail)
                {
                    throw new NotSupportedException($"{nameof(AccountController)} requires a user store with email support.");
                }

                var userStore = sp.GetRequiredService<IUserStore<ApplicationUser>>();
                var emailStore = (IUserEmailStore<ApplicationUser>)userStore;
                var email = payload.Email;

                if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
                {
                    return BadRequest(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
                }

                var user = new ApplicationUser();
                await userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, email, CancellationToken.None);
                var result = await userManager.CreateAsync(user, Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmResult = await userManager.ConfirmEmailAsync(user, code);

                if (!confirmResult.Succeeded)
                {
                    return BadRequest(confirmResult.Errors);
                }

                var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();

                var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                var isPersistent = (useCookies == true) && (useSessionCookies != true);
                signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

                await signInManager.SignInAsync(user, isPersistent: false);

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                return Problem("Error: " + ex.Message, statusCode: StatusCodes.Status401Unauthorized);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp)
        {
            try
            {
                var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();

                var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                var isPersistent = (useCookies == true) && (useSessionCookies != true);
                signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

                var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, lockoutOnFailure: true);

                if (result.RequiresTwoFactor)
                {
                    if (!string.IsNullOrEmpty(login.TwoFactorCode))
                    {
                        result = await signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);
                    }
                    else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                    {
                        result = await signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
                    }
                }

                if (!result.Succeeded)
                {
                    return Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
                }

                // The signInManager should have returned the token when PasswordSignInAsync was called.
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in the Login endpoint: {ex.Message}");
                return Problem("An error occurred during login.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleTokenData tokenData, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp)
        {
            try
            {

                var clientId = _configuration["GOOGLE_CLIENT_ID"];
                var clientSecret = _configuration["GOOGLE_CLIENT_SECRET"];
                var redirectUri = _configuration["GOOGLE_REDIRECT_URI"];
                var oauthApiAddress = _configuration["GOOGLE_OAUTH_API_ADDRESS"];

                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "code", tokenData.Credential },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                    
                });

                var response = await httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("responseString is " + responseString + "\n\n");
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);

                var idToken = responseObject.id_token.ToString();
                Console.WriteLine("Credential is " + idToken);

                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                if (payload != null)
                {
                    var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

                    var user = await userManager.FindByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        Console.WriteLine("User not registered");
                        return Problem("User Not registered ");
                    }

                    var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();
                    var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                    var isPersistent = (useCookies == true) && (useSessionCookies != true);
                    signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

                    await signInManager.SignInAsync(user, isPersistent: false);

                    Console.WriteLine("\n\n ive supposedly signed in google user\n\n");

                    // The signInManager should have returned the token when PasswordSignInAsync was called.
                    return new EmptyResult();
                }
                else
                {
                    return Problem("failed google validation ");
                }
            }
            catch (Exception ex)
            {
                return Problem("failed google validation " + ex);
            }
        }

        [HttpGet("confirmEmail" , Name = "ConfirmEmailRoute")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string code, [FromQuery] string? changedEmail, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync(email) is not { } user)
            {
                
                Console.WriteLine("user not found email is : " + email);
                return Unauthorized();
            }

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error occurred while decoding the code: {ex.Message}"); // Log the detailed error message
                return Unauthorized();
            }

            IdentityResult result;

            if (string.IsNullOrEmpty(changedEmail))
            {
                try
                {

                        var confirmResult = await userManager.ConfirmEmailAsync(user, code);
                        if (!confirmResult.Succeeded)
                        {
                            foreach (var error in confirmResult.Errors)
                            {
                                Console.WriteLine($"Error: {error.Description}");
                            }
                            return BadRequest(confirmResult.Errors);
                        }

                        var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();

                        var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                        var isPersistent = (useCookies == true) && (useSessionCookies != true);
                        signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;
                        await signInManager.SignInAsync(user, isPersistent);


                    // The signInManager already produced the cookie or token.
                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    return Problem("An error occurred: " + ex.Message);
                }
            }
            else
            {
                //Identity, email and username are the same, when we update the email, we need to update the user name.
                result = await userManager.ChangeEmailAsync(user, changedEmail, code);

                if (result.Succeeded)
                {
                    result = await userManager.SetUserNameAsync(user, changedEmail);
                }

                return Ok();
            }

            return Problem("An error occurred: shouldnt of got here");
        }

        [HttpPost("resendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendRequest resendRequest, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(resendRequest.email);
            if (user is null)
            {
                return NotFound(new { message = "User not found." }); 
            }

            try
            {
                var confirmationCode = await ConfirmationEmailAsync(user, HttpContext, resendRequest.email);
                
                return Ok(new { message = "Confirmation email sent successfully.", confirmationCode }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
            }
        }


        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResendRequest resetRequest, [FromServices] IServiceProvider sp)
        {
            try
            {
                var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByEmailAsync(resetRequest.email);

                if (user is null)
                {
                    Console.Write($"User not found\n\n");
                    return Ok();
                }

                if (!await userManager.IsEmailConfirmedAsync(user))
                {
                    Console.Write($"Email not confirmed\n\n");
                    return Ok();
                }

                var confirmationCode = await userManager.GeneratePasswordResetTokenAsync(user);
                confirmationCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));

                //Send code to email Disabled for live demo, code is sent to frontend  abd displayed in console
                // var emailSender = new AWSEmailSender(_configuration);
                // await emailSender.SendPasswordResetCodeAsync(user, HtmlEncoder.Default.Encode(code), "callback-url");
                Console.WriteLine("Password reset code created, code: " + confirmationCode);
                return Ok(new { message = "Confirmation email sent successfully.", confirmationCode }); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ForgotPassword: {ex.Message}");
                return Problem("An error occurred during password reset.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetRequest, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(resetRequest.Email);

            if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                Console.Write($"User not found");
                // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                // returned a 400 for an invalid code given a valid user email.
                return BadRequest(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
            }

            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
                result = await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }

                Console.WriteLine("Reset password request completed");
                return Ok();
            }
            catch (FormatException)
            {
                Console.Write($"attempt to reset flopped likely code? not found\n\n");

                return BadRequest(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
            }
            return Ok();
        }

        // [HttpPost("2fa")]
        // public async Task<IActionResult> TwoFactorAuthentication(ClaimsPrincipal claimsPrincipal, [FromBody] TwoFactorRequest tfaRequest, [FromServices] IServiceProvider sp)
        // {   
        //     //TODO find fix for [FromBody] ClaimsPrincipal claimsPrincipal, as input arg and check if is user
        //     //cant get user info because not authenticated yet?? 
        //     var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();
        //     var userManager = signInManager.UserManager;
        //     // if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        //     // {
        //     //     return NotFound();
        //     // }

        //     if (tfaRequest.Enable == true)
        //     {
        //         if (tfaRequest.ResetSharedKey)
        //         {
        //             //"Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated."
        //             return BadRequest("CannotResetSharedKeyAndEnable");
        //         }
        //         else if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
        //         {
        //             //"No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa."
        //             return BadRequest("RequiresTwoFactor");
        //         }
        //         else if (!await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
        //         {
        //             //"The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa."
        //             return BadRequest("InvalidTwoFactorCode");
        //         }

        //         await userManager.SetTwoFactorEnabledAsync(user, true);
        //     }
        //     else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
        //     {
        //         await userManager.SetTwoFactorEnabledAsync(user, false);
        //     }

        //     if (tfaRequest.ResetSharedKey)
        //     {
        //         await userManager.ResetAuthenticatorKeyAsync(user);
        //     }

        //     string[]? recoveryCodes = null;
        //     if (tfaRequest.ResetRecoveryCodes || (tfaRequest.Enable == true && await userManager.CountRecoveryCodesAsync(user) == 0))
        //     {
        //         var recoveryCodesEnumerable = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        //         recoveryCodes = recoveryCodesEnumerable?.ToArray();
        //     }

        //     if (tfaRequest.ForgetMachine)
        //     {
        //         await signInManager.ForgetTwoFactorClientAsync();
        //     }

        //     var key = await userManager.GetAuthenticatorKeyAsync(user);
        //     if (string.IsNullOrEmpty(key))
        //     {
        //         await userManager.ResetAuthenticatorKeyAsync(user);
        //         key = await userManager.GetAuthenticatorKeyAsync(user);

        //         if (string.IsNullOrEmpty(key))
        //         {
        //             throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
        //         }
        //     }

        //     return Ok(new TwoFactorResponse
        //     {
        //         SharedKey = key,
        //         RecoveryCodes = recoveryCodes,
        //         RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
        //         IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
        //         IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
        //     });
        // }

        
        [HttpGet("info")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetUserInfo([FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(HttpContext.User);

            //TODO delete because has already passed through the authorization middleware successfully, checking if the user is null in this context is redundant
            if (user == null)
            {
                return NotFound();
            }

            return Ok(await CreateInfoResponseAsync(user));
        }

        [HttpPost("info")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] InfoRequest infoRequest, [FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(HttpContext.User);

             //TODO delete because has already passed through the authorization middleware successfully, checking if the user is null in this context is redundant
            if (user == null)
            {
                Console.WriteLine("user not found");
                return BadRequest("error user not found");
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
            {
                return BadRequest(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
            }

            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
            {
                if (string.IsNullOrEmpty(infoRequest.OldPassword))
                {
                    return BadRequest("The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
                }

                var changePasswordResult = await userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return BadRequest(changePasswordResult);
                }
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
            {
                var email = await userManager.GetEmailAsync(user);

                if (email != infoRequest.NewEmail)
                {
                    await ConfirmationEmailAsync(user, HttpContext, infoRequest.NewEmail, isChange: true);
                }
            }

            return Ok(await CreateInfoResponseAsync(user));
        }


        [HttpPost("signupteacher")] 
        public async Task<IActionResult> SignUpTeacher([FromBody] AdminCreateTeacherData model,  [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                user.Name = model.name;
                user.Dob = model.dob.HasValue ? model.dob.Value.ToUniversalTime() : (DateTime?)null;
                user.PhoneNumber = model.PhoneNumber;
                //user.SortCode = model.sortCode;
                //user.AccountNo = model.accountNo;
                user.Stream = model.stream;
                user.Notes = model.notes;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest("Unable to update user");
                }

                var teacher = new Teacher { teacherId = user.Id, name = user.Name, notes = user.Notes};
                _dbcontext.teachers.Add(teacher);
                
                
                var addRoleResult = await _userManager.AddToRoleAsync(user, "Teacher");
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add user to role.");
                }
                
                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine("stage two sign up, user and role created, about to signin send token.");

                var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();

                var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                var isPersistent = (useCookies == true) && (useSessionCookies != true);
                signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

                await signInManager.SignInAsync(user, isPersistent: false);

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the teacher: {ex.Message}");

                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback failed: {rollbackEx.Message}");
                    return BadRequest($"Error creating user: {ex.Message}. Attempted Rollback failed: {rollbackEx.Message}");
                }

                return BadRequest($"Error: {ex.Message}. Transaction rolled back.");
            }
        }


        [HttpPost("signupstudent")]
        public async Task<IActionResult> SignUpStudent([FromBody] AdminCreateStudentData model, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                user.Name = model.name;
                user.Dob = model.dob.HasValue ? model.dob.Value.ToUniversalTime() : (DateTime?)null;
                user.PhoneNumber = model.PhoneNumber;
                user.Stream = model.stream;
                user.Notes = model.notes;
                user.ParentName = model.parentName;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest("Unable to update user");
                }

                // Find the first teacher in the database
                var firstTeacher = await _dbcontext.teachers.FirstOrDefaultAsync();
                if (firstTeacher == null)
                {
                    return BadRequest("No teachers found in the database");
                }

                var student = new Student { studentId = user.Id, teacherId = firstTeacher.teacherId, name = user.Name, notes = user.Notes, stream = user.Stream };
                _dbcontext.students.Add(student);

                var addRoleResult = await _userManager.AddToRoleAsync(user, "Student");
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add user to role.");
                }

                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                var didAssign = await AssignAssessmentInternal(userId, firstTeacher.teacherId, 1);
                if (didAssign)
                {
                    Console.WriteLine("Assessment assigned successfully");
                }
                else
                {
                    Console.WriteLine("Unable to assign initial assessment");
                }
                Console.WriteLine("stage two sign up, user and role created, about to signin send token");

                var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();

                var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                var isPersistent = (useCookies == true) && (useSessionCookies != true);
                signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

                await signInManager.SignInAsync(user, isPersistent: false);

                // The signInManager already produced the cookie or token.
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback failed: {rollbackEx.Message}");
                    return BadRequest($"Error creating user stage 2: {ex.Message}. Attempted Rollback failed: {rollbackEx.Message}");
                }

                return BadRequest($"Error: {ex.Message}. Transaction rolled back.");
            }
        }

        [HttpPost("logout")][Authorize]
        public async Task<IActionResult> Logout([FromBody]object empty)
        {
            if (empty != null)
            {
                // only for cookies, TODO create revoked list
                await _signInManager.SignOutAsync();
                return Ok(new { message = "User logged out successfully" });
            }
            return BadRequest("empty obj not recieved");
        }

        public async Task<string> ConfirmationEmailAsync(ApplicationUser user, HttpContext context, string email, bool isChange = false)
        {
            try
            {
                var timeProvider = context.RequestServices.GetRequiredService<TimeProvider>();
                var bearerTokenOptions = context.RequestServices.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
                var emailSender = context.RequestServices.GetRequiredService<IEmailSender<ApplicationUser>>();
                var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
                
                if (confirmEmailEndpointName is null)
                {
                    throw new NotSupportedException("No email confirmation endpoint was registered!");
                }

                var code = isChange
                    ? await _userManager.GenerateChangeEmailTokenAsync(user, email)
                    : await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                Console.WriteLine($"\n\n email confirmation code is: {code}");

                var userId = await _userManager.GetUserIdAsync(user);

                var routeValues = new RouteValueDictionary()
                {
                    ["userId"] = userId,
                    ["code"] = code,
                };

                if (isChange)
                {
                    routeValues.Add("changedEmail", email);
                }

                var confirmEmailUrl = linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
                ?? throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");

                var urlnew = HtmlEncoder.Default.Encode(confirmEmailUrl);
                Console.WriteLine("\n\n email confirmation link is: \n\n" + urlnew);

                await emailSender.SendConfirmationLinkAsync(user, email, urlnew);

                return code; // Return the confirmation code
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating confirmEmailUrl: {ex.Message}");
                throw;
            }
        }



        private async Task<InfoResponse> CreateInfoResponseAsync(ApplicationUser user)
        {
            return new()
            {
                Email = await _userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
            };
        }

        
        private async Task<bool> AssignAssessmentInternal(string studentId, string teacherId, int assessment_id)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    string title = null;

                    switch (assessment_id)
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


                     // Set a future date for all assessments
                     DateTime futureDate = DateTime.UtcNow.AddDays(30);

                    var assignment = new StudentAssessment
                    {
                        studentId = studentId,
                        teacherId = teacherId,
                        assessmentId = assessment_id,
                        isAssigned = true,
                        isSubmitted = true,
                        isGraded = true,
                        dueDate = futureDate
                    };

                    _dbcontext.student_assessment.Add(assignment);
                    await _dbcontext.SaveChangesAsync();

                    var assignmentId = assignment.id ?? 0; 

                    if (assignmentId == 0)
                    {
                        Console.WriteLine("Lesson ID cannot be null unable to assign assessment");
                        return false;
                    }

                    var CalendarEvent = new CalendarEvent
                    {
                        eventId = assignmentId,
                        studentId = studentId,
                        teacherId = teacherId, 
                        title = title,
                        description = "formal assessment",
                        status = "pending",
                        date = futureDate
                    };
                    _dbcontext.calendar_events.Add(CalendarEvent);
                    await _dbcontext.SaveChangesAsync();

                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"An error occurred while creating the assessment: {ex.Message}");
                    
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    
                    return false;
                }
            }
        }
    }
}