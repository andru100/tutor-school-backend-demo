using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;


using System.Collections.Generic;
using Microsoft.AspNetCore;
using Stripe;
using Stripe.Checkout;





using AWSEmail;

using Model;
using seed;

namespace Main
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Console.WriteLine("DB_HOST: " + Configuration["DB_HOST"]);
            Console.WriteLine("DB_PORT: " + Configuration["DB_PORT"]);
            Console.WriteLine("DB_USERNAME: " + Configuration["DB_USERNAME"]);
            Console.WriteLine("DB_PASSWORD: " + Configuration["DB_PASSWORD"]);
            Console.WriteLine("DB_NAME: " + Configuration["DB_NAME"]);
        }

        public async void ConfigureServices(IServiceCollection services)
        {   
            Console.WriteLine("allowed cors origin is " + Configuration["ALLOWED_CORS_ORIGIN"]);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    // policy.WithOrigins(Configuration["ALLOWED_CORS_ORIGIN"])
                    //     .AllowAnyHeader()
                    //     .AllowAnyMethod()
                    //     .AllowCredentials();

                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            
            //string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Username={Environment.GetEnvironmentVariable("DB_USERNAME")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};Database={Environment.GetEnvironmentVariable("DB_NAME")};";
            string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Username={Environment.GetEnvironmentVariable("DB_USERNAME")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};Database={Environment.GetEnvironmentVariable("DB_NAME")};IncludeErrorDetail=true;";
            Console.WriteLine("connection string is: " + connectionString);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            services.AddHttpContextAccessor();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddAuthentication();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("Teacher", policy =>
                      policy.RequireAssertion(context =>
                          context.User.IsInRole("Teacher") || 
                          context.User.IsInRole("Admin")));
                options.AddPolicy("Student", policy =>
                      policy.RequireAssertion(context =>
                          context.User.IsInRole("Teacher") || 
                          context.User.IsInRole("Student") ||
                          context.User.IsInRole("Admin")));
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddIdentityApiEndpoints<ApplicationUser>(options =>
            //services.addDefaultidentity<ApplicationUser>(options =>
            {
                // Require confirmed email
                //options.SignIn.RequireConfirmedEmail = true;
            })
            
                .AddRoles<IdentityRole>()
                
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddHttpClient();  
            

            services.AddScoped<AWSEmailSender>();  
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                StripeConfiguration.ApiKey = Configuration["STRIPE_API_KEY"];


                app.UseRouting();
                app.UseCors();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch (Exception ex)
            {  
                Console.WriteLine($"Unexpected error on startup Configure: {ex.Message}");
            }
        }
    }
}