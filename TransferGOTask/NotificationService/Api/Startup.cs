using Amazon.SimpleEmail.Model;
using TransferGOTask.NotificationService.Infrastructure.Mappers;

namespace TransferGOTask.NotificationService.Api;

using Amazon.Runtime;
using Infrastructure.Services;
using Infrastructure.Providers.Email;
using Infrastructure.Providers.Sms;
using System.Net.Mail;
using Twilio;
using Amazon.SimpleNotificationService;
using Amazon.SimpleEmail;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;
using Domain.Services;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Infrastructure.Resilience;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        // Settings 
        services.Configure<NotificationSettings>(Configuration.GetSection("NotificationSettings"));
        services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));
        
        // Database
        services.AddDbContext<NotificationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("NotificationsDb")));
        
        // Repos + Services
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, Application.Services.NotificationService>();
        services.AddScoped<NotificationDispatcher>();
        
        // Resilience
        services.AddSingleton<RetryPolicyFactory>();
        services.AddSingleton<IFailoverStrategy, FailoverPolicy>();
        
        // Mappers
        services.AddSingleton<INotificationRequestMapper, NotificationRequestMapper>(); 
        services.AddSingleton<INotificationRequestDtoMapper, NotificationRequestDtoMapper>();
        
        // AWS Services using localstack
        var awsOptions = Configuration.GetAWSOptions("AWS");
        var awsSettings = Configuration.GetSection("AWS");
        var awsCreds = new BasicAWSCredentials("localstack", "localstack");
        services.AddDefaultAWSOptions(awsOptions);
        
        services.AddSingleton<IAmazonSimpleNotificationService>(_ =>
            new AmazonSimpleNotificationServiceClient(
                awsCreds,
                new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = awsSettings["ServiceUrl"],
                    AuthenticationRegion = awsSettings["Region"]
                }));
        services.AddSingleton<IAmazonSimpleEmailService>(_ => 
            new AmazonSimpleEmailServiceClient(
                awsCreds,
                new AmazonSimpleEmailServiceConfig {
                    ServiceURL           = awsSettings["ServiceUrl"],
                    AuthenticationRegion = awsSettings["Region"]
                }));
        
        // SMTP Client
        services.AddSingleton(sp =>
        {
            var smtpCfg = Configuration.GetSection("Smtp");
            var client = new SmtpClient(
                smtpCfg["Host"],
                int.Parse(smtpCfg["Port"])
            )
            {
                EnableSsl = bool.Parse(smtpCfg["EnableSsl"])
            };
            return client;
        });
        
        // Twilio
        var tw = Configuration
                     .GetSection("Twilio")
                     .Get<TwilioSettings>();
        TwilioClient.Init(tw.AccountSid, tw.AuthToken);
        
        // Notification Providers
        services.AddTransient<INotificationProvider, TwilioSmsProvider>();  
        services.AddTransient<INotificationProvider, SnsSmsProvider>();     
        services.AddTransient<INotificationProvider, SmtpEmailProvider>();  
        services.AddTransient<INotificationProvider, SesEmailProvider>();   

        // Background worker
        services.AddHostedService<NotificationQueueService>();
        
        services.AddControllers();
        services.AddSwaggerGen();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            if (env.IsDevelopment())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
            else
            {
                db.Database.Migrate();
            }
        }
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService v1"));
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

