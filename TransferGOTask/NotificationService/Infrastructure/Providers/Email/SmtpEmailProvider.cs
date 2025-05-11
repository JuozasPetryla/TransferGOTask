namespace TransferGOTask.NotificationService.Infrastructure.Providers.Email;

using System.Net.Mail;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Configuration;
using Resilience;

public class SmtpEmailProvider : BaseEmailProvider
{
    private readonly SmtpClient _smtp;
    public override string Key => "SMTP";

    public SmtpEmailProvider(
        SmtpClient smtp,
        RetryPolicyFactory retryFactory,
        IOptions<NotificationSettings> settings)
        : base(retryFactory, settings)
    {
        _smtp = smtp;
    }

    protected override Task SendEmailAsync(Notification notification, CancellationToken cancellationToken)
    {
        var mail = new MailMessage("no-reply@myapp.com", notification.Recipient.Email)
        {
            Subject = notification.Message.Subject,
            Body = notification.Message.Body,
            IsBodyHtml = false
        };
        return _smtp.SendMailAsync(mail);
    }
}